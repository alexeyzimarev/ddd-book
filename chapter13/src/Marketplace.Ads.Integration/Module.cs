using System;
using EventStore.ClientAPI;
using Marketplace.Ads.Integration.ClassifiedAds;
using Marketplace.EventSourcing;
using Marketplace.EventStore;
using Marketplace.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Marketplace.Ads.Integration
{
    public static class AdsIntegrationModule
    {
        const string SubscriptionName = "adsIntegrationSubscription";

        public static IMvcCoreBuilder AddAdsModule(
            this IMvcCoreBuilder builder,
            string databaseName
        )
        {
            EventMappings.MapEventTypes();

            builder.Services.AddSingleton(
                c =>
                {
                    var store = c.GetRavenStore();
                    store.CheckAndCreateDatabase(databaseName);

                    IAsyncDocumentSession GetSession()
                        => c.GetRavenStore()
                            .OpenAsyncSession(databaseName);

                    var connection = c.GetEsConnection();
                    var eventStore = c.GetEventStore();

                    return new SubscriptionManager(
                        connection,
                        new EsCheckpointStore(
                            connection, SubscriptionName
                        ),
                        SubscriptionName,
                        new EventStoreReactor(
                            e => AdsReaction.React(eventStore, GetSession, e)
                        )
                    );
                }
            );

            return builder;
        }

        static IDocumentStore GetRavenStore(
            this IServiceProvider provider
        )
            => provider.GetRequiredService<IDocumentStore>();

        static IEventStore GetEventStore(
            this IServiceProvider provider
        )
            => provider.GetRequiredService<IEventStore>();

        static IEventStoreConnection GetEsConnection(
            this IServiceProvider provider
        )
            => provider.GetRequiredService<IEventStoreConnection>();
    }
}
