using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Domain.ClassifiedAds.Events;
using static Marketplace.PaidServices.Queries.ClassifiedAds.ReadModels;

namespace Marketplace.PaidServices.Queries.ClassifiedAds
{
    public static class ActiveServicesProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event
        )
        {
            return @event switch
            {
                V1.OrderCreated e => ()
                    => session.Create(e.FromCreated),
                V1.ServiceActivated e => ()
                    => UpdateActiveServices(
                        e.ClassifiedAdId,
                        services => services.Add(
                            new AdActiveServices.Service
                                {ServiceType = e.ServiceType}
                        )
                    ),
                V1.ServiceDeactivated e => ()
                    => UpdateActiveServices(
                        e.ClassifiedAdId,
                        services => services.RemoveAll(
                            x => x.ServiceType == e.ServiceType
                        )
                    ),
                _ => (Func<Task>) null
            };

            Task UpdateActiveServices(
                Guid id,
                Action<List<AdActiveServices.Service>> update
            )
                => session.Update<AdActiveServices>(
                    AdActiveServices.GetDatabaseId(id),
                    x => update(x.ActiveServices)
                );
        }

        static AdActiveServices FromCreated(this V1.OrderCreated @event)
            => new AdActiveServices
            {
                ClassifiedAdId = @event.ClassifiedAdId,
                ActiveServices = new List<AdActiveServices.Service>()
            };
    }
}
