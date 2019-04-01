using System;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.Infrastructure.Currency;
using Marketplace.Infrastructure.EventStore;
using Marketplace.Infrastructure.Profanity;
using Marketplace.Infrastructure.RavenDb;
using Marketplace.Infrastructure.Vue;
using Marketplace.Modules.Auth;
using Marketplace.Modules.ClassifiedAds;
using Marketplace.Modules.FunctionalAd;
using Marketplace.Modules.Images;
using Marketplace.Modules.Projections;
using Marketplace.Modules.UserProfile;
using Marketplace.Modules.UserProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Swashbuckle.AspNetCore.Swagger;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

// ReSharper disable UnusedMember.Global

[assembly: ApiController]

namespace Marketplace
{
    public class Startup
    {
        public const string CookieScheme = "MarketplaceScheme";

        public Startup(
            IHostingEnvironment environment,
            IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }
        IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Modules.ClassifiedAds.EventMappings.MapEventTypes();
            Modules.UserProfile.EventMappings.MapEventTypes();

            var esConnection = EventStoreConnection.Create(
                Configuration["eventStore:connectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName
            );
            var store = new EsAggregateStore(esConnection);
            var purgomalumClient = new PurgomalumClient();

            var documentStore = ConfigureRavenDb(
                Configuration["ravenDb:server"],
                Configuration["ravenDb:database"]
            );

            IAsyncDocumentSession GetSession() => documentStore.OpenAsyncSession();

            services.AddSingleton(
                new ClassifiedAdsCommandService(
                    store, 
                    new FixedCurrencyLookup(),
                    ImageStorage.UploadFile
                )
            );

            services.AddSingleton(
                new TestAdCommandService(new TestStore(esConnection))
            );

            services.AddSingleton(
                new UserProfileCommandService(
                    store, t => purgomalumClient.CheckForProfanity(t)
                )
            );

            services.AddSingleton(new ImageQueryService(ImageStorage.GetFile));

            var ravenDbProjectionManager = new ProjectionManager(
                esConnection,
                new RavenDbCheckpointStore(GetSession, "readmodels"),
                ConfigureRavenDbProjections(GetSession)
            );

            var upcasterProjectionManager = new ProjectionManager(
                esConnection,
                new EsCheckpointStore(esConnection, "upcaster"),
                ConfigureUpcasters(esConnection, GetSession)
            );

            services.AddSingleton(c => (Func<IAsyncDocumentSession>) GetSession);
            services.AddSingleton(c => new AuthService(GetSession));
            services.AddScoped(c => GetSession());

            services.AddSingleton<IHostedService>(
                new EventStoreService(
                    esConnection,
                    ravenDbProjectionManager,
                    upcasterProjectionManager
                )
            );

            services
                .AddAuthentication(
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
                .AddCookie();

            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSpaStaticFiles(
                configuration =>
                    configuration.RootPath = "ClientApp/dist"
            );

            services.AddSwaggerGen(
                c =>
                    c.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "ClassifiedAds", Version = "v1"
                        }
                    )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseMvc(
                routes =>
                {
                    routes.MapRoute(
                        "default",
                        "{controller=Home}/{action=Index}/{id?}"
                    );

                    routes.MapRoute(
                        "api",
                        "api/{controller=Home}/{action=Index}/{id?}"
                    );

                    routes.MapSpaFallbackRoute(
                        "spa-fallback",
                        new {controller = "Home", action = "Index"}
                    );
                }
            );

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(
                c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json", "ClassifiedAds v1"
                )
            );

            app.UseSpa(
                spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                        spa.UseVueDevelopmentServer("serve:bs");
                }
            );
        }

        static IDocumentStore ConfigureRavenDb(
            string serverUrl,
            string database)
        {
            var store = new DocumentStore
            {
                Urls = new[] {serverUrl},
                Database = database
            };
            store.Initialize();

            var record = store.Maintenance.Server.Send(
                new GetDatabaseRecordOperation(store.Database)
            );

            if (record == null)
                store.Maintenance.Server.Send(
                    new CreateDatabaseOperation(
                        new DatabaseRecord(store.Database)
                    )
                );

            return store;
        }

        static IProjection[] ConfigureRavenDbProjections(
            Func<IAsyncDocumentSession> getSession)
            => new IProjection[]
            {
                new RavenDbProjection<ReadModels.ClassifiedAdDetails>(
                    getSession,
                    (session, @event) =>
                        ClassifiedAdDetailsProjection.GetHandler(
                            session, @event,
                            userId =>
                                getSession.GetUserDetails(
                                    userId, x => x.DisplayName
                                )
                        )
                ),
                new RavenDbProjection<ReadModels.UserDetails>(
                    getSession, UserDetailsProjection.GetHandler
                ),
                new RavenDbProjection<ReadModels.MyClassifiedAds>(
                    getSession, MyClassifiedAdsProjection.GetHandler
                )
            };

        static IProjection[] ConfigureUpcasters(
            IEventStoreConnection connection,
            Func<IAsyncDocumentSession> getSession)
            => new IProjection[]
            {
                new ClassifiedAdUpcasters(
                    connection,
                    userId => getSession.GetUserDetails(
                        userId, x => x.PhotoUrl
                    )
                )
            };
    }
}