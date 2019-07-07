using System;
using System.Runtime.CompilerServices;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.EventStore;
using Marketplace.RavenDb;
using Marketplace.Users.Auth;
using Marketplace.Users.Domain.Shared;
using Marketplace.Users.Projections;
using Marketplace.Users.UserProfiles;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static Marketplace.Users.Projections.ReadModels;

namespace Marketplace.Users
{
    public static class UsersModule
    {
        const string SubscriptionName = "usersSubscription";

        public static IMvcCoreBuilder AddUsersModule(
            this IMvcCoreBuilder builder,
            string databaseName,
            CheckTextForProfanity profanityCheck
        )
        {
            EventMappings.MapEventTypes();

            builder.Services.AddSingleton(
                    c =>
                        new UserProfileCommandService(
                            c.GetAggregateStore(),
                            profanityCheck
                        )
                )
                .AddSingleton<GetUsersModuleSession>(
                    c =>
                    {
                        var store = c.GetRequiredService<IDocumentStore>();
                        store.CheckAndCreateDatabase(databaseName);

                        IAsyncDocumentSession GetSession()
                            => store.OpenAsyncSession(databaseName);

                        return GetSession;
                    }
                )
                .AddSingleton(
                    c =>
                    {
                        var getSession =
                            c.GetRequiredService<GetUsersModuleSession>();

                        return new SubscriptionManager(
                            c.GetEsConnection(),
                            new RavenDbCheckpointStore(
                                () => getSession(),
                                SubscriptionName
                            ),
                            SubscriptionName,
                            StreamName.AllStream,
                            new RavenDbProjection<UserDetails>(
                                () => getSession(),
                                UserDetailsProjection.GetHandler
                            )
                        );
                    }
                )
                .AddSingleton<AuthService>();

            builder.AddApplicationPart(typeof(UsersModule).Assembly);

            return builder;
        }

        static IEventStoreConnection GetEsConnection(
            this IServiceProvider provider
        )
            => provider.GetRequiredService<IEventStoreConnection>();

        static IAggregateStore GetAggregateStore(this IServiceProvider provider)
            => provider.GetRequiredService<IAggregateStore>();
    }

    public delegate IAsyncDocumentSession GetUsersModuleSession();
}
