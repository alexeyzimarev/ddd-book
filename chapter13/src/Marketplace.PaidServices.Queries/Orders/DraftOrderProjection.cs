using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.PaidServices.Domain.Orders;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;

namespace Marketplace.PaidServices.Queries.Orders
{
    public static class DraftOrderProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event
        )
        {
            return @event switch
            {
                Events.V1.OrderCreated e =>
                    () => session.Create<ReadModels.OrderDraft>(
                        x =>
                        {
                            x.Id = DbId(e.OrderId);
                            x.CustomerId = e.CustomerId.ToString();
                            x.ClassifiedAdId = e.ClassifiedAdId.ToString();
                            x.Services = new List<ReadModels.OrderDraft.Service>();
                        }
                    ),
                Events.V1.ServiceAddedToOrder e =>
                    () => session.Update<ReadModels.OrderDraft>(
                        DbId(e.OrderId),
                        x => x.Services.Add(
                            new ReadModels.OrderDraft.Service
                            {
                                Type = e.ServiceType,
                                Description = e.Description,
                                Price = e.Price
                            }
                        )
                    ),
                Events.V1.ServiceRemovedFromOrder e =>
                    () => session.Update<ReadModels.OrderDraft>(
                        DbId(e.OrderId),
                        x => x.Services.RemoveAll(s => s.Type == e.ServiceType)
                    ),
                Events.V1.OrderFulfilled e =>
                    () => session.Del(DbId(e.OrderId)),
                Events.V1.OrderDeleted e =>
                    () => session.Del(DbId(e.OrderId)),
                _ => (Func<Task>) null
            };

            string DbId(Guid guid) => ReadModels.OrderDraft.GetDatabaseId(guid);
        }
    }
}