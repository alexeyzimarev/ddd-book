using EventStore.ClientAPI;
using Marketplace.Ads.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Projections;
using Marketplace.EventStore;
using Marketplace.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Projections.ReadModels;

namespace Marketplace.Ads
{
    public static class AdsModule
    {
        const string SubscriptionName = "adsSubscription";

        public static IMvcCoreBuilder AddAdsModule(
            this IMvcCoreBuilder builder,
            string databaseName,
            ICurrencyLookup currencyLookup,
            UploadFile uploadFile
        )
        {
            builder.Services.AddSingleton(
                c =>
                    new ClassifiedAdsCommandService(
                        new EsAggregateStore(
                            c.GetRequiredService<IEventStoreConnection>()
                        ),
                        currencyLookup,
                        uploadFile
                    )
            );

            builder.Services.AddSingleton(
                c =>
                {
                    var store = c.GetRequiredService<IDocumentStore>();
                    store.CheckAndCreateDatabase(databaseName);
                    
                    IAsyncDocumentSession GetSession()
                        => c.GetRequiredService<IDocumentStore>()
                            .OpenAsyncSession(databaseName);

                    return new SubscriptionManager(
                        c.GetRequiredService<IEventStoreConnection>(),
                        new RavenDbCheckpointStore(
                            GetSession, SubscriptionName
                        ),
                        SubscriptionName,
                        new RavenDbProjection<ClassifiedAdDetails>(
                            GetSession,
                            ClassifiedAdDetailsProjection.GetHandler
                        ),
                        new RavenDbProjection<MyClassifiedAds>(
                            GetSession,
                            MyClassifiedAdsProjection.GetHandler
                        )
                    );
                }
            );

            builder.AddApplicationPart(typeof(AdsModule).Assembly);

            return builder;
        }
    }
}