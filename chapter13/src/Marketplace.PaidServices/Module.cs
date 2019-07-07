using System;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.EventStore;
using Marketplace.PaidServices.ClassifiedAds;
using Marketplace.PaidServices.Integration;
using Marketplace.PaidServices.Orders;
using Marketplace.PaidServices.Queries.ClassifiedAds;
using Marketplace.PaidServices.Queries.Orders;
using Marketplace.PaidServices.Reactors;
using Marketplace.RavenDb;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Queries.Orders.ReadModels;
using ReadModels = Marketplace.PaidServices.Queries.ClassifiedAds.ReadModels;

namespace Marketplace.PaidServices
{
    public static class PaidServicesModule
    {
        public static IMvcCoreBuilder AddPaidServicesModule(
            this IMvcCoreBuilder builder,
            string databaseName
        )
        {
            EventMappings.MapEventTypes();

            builder.Services.AddSingleton(
                c => new OrdersCommandService(c.GetStore())
            );

            builder.Services.AddSingleton(
                c => new ClassifiedAdCommandService(c.GetStore())
            );

            builder.Services.AddSingleton(
                c =>
                {
                    var store = c.GetRequiredService<IDocumentStore>();
                    store.CheckAndCreateDatabase(databaseName);
                    const string subscriptionName = "servicesReadModels";

                    IAsyncDocumentSession GetSession()
                        => c.GetRequiredService<IDocumentStore>()
                            .OpenAsyncSession(databaseName);

                    return new SubscriptionManager(
                        c.GetRequiredService<IEventStoreConnection>(),
                        new RavenDbCheckpointStore(
                            GetSession, subscriptionName
                        ),
                        subscriptionName,
                        StreamName.AllStream,
                        new RavenDbProjection<OrderDraft>(
                            GetSession,
                            DraftOrderProjection.GetHandler
                        ),
                        new RavenDbProjection<CompletedOrder>(
                            GetSession,
                            CompletedOrderProjection.GetHandler
                        ),
                        new RavenDbProjection<ReadModels.AdActiveServices>(
                            GetSession,
                            ActiveServicesProjection.GetHandler
                        )
                    );
                }
            );

            builder.Services.AddSingleton(
                c =>
                {
                    var service =
                        c.GetRequiredService<ClassifiedAdCommandService>();

                    var connection =
                        c.GetRequiredService<IEventStoreConnection>();
                    const string subscriptionName = "servicesReactors";

                    return new SubscriptionManager(
                        connection,
                        new EsCheckpointStore(connection, subscriptionName),
                        subscriptionName,
                        StreamName.Custom(StreamNames.AdsIntegrationStream),
                        new EventStoreReactor(
                            x => OrderReaction.React(service, x)
                        )
                    );
                }
            );

            builder.AddApplicationPart(typeof(PaidServicesModule).Assembly);

            return builder;
        }

        static IFunctionalAggregateStore GetStore(
            this IServiceProvider provider
        )
            => new FunctionalStore(
                provider.GetRequiredService<IEventStore>()
            );
    }
}
