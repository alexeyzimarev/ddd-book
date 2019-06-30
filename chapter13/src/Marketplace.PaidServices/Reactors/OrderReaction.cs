using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.PaidServices.ClassifiedAds;
using Marketplace.PaidServices.Domain.Orders;
using V1 = Marketplace.PaidServices.ClassifiedAds.Commands.V1;

namespace Marketplace.PaidServices.Reactors
{
    public static class OrderReaction
    {
        public static Func<Task> React(
            ClassifiedAdCommandService service,
            object @event
        )
            => @event switch
            {
                Events.V1.OrderCreated e =>
                    () => service.Handle(
                        new V1.Create
                        {
                            ClassifiedAdId = e.ClassifiedAdId,
                            SellerId = e.CustomerId
                        }
                    ),
                Events.V1.OrderFulfilled e =>
                    () => service.Handle(
                        new V1.FulfillOrder
                        {
                            ClassifiedAdId = e.ClassifiedAdId,
                            When = e.FulfilledAt,
                            ServiceTypes = e.Services
                                .Select(x => x.Type)
                                .ToArray()
                        }
                    ),
                _ => (Func<Task>) null
            };
    }
}