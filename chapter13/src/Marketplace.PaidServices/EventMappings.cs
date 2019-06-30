using static Marketplace.EventSourcing.TypeMapper;
using static Marketplace.PaidServices.Domain.ClassifiedAds.Events.V1;
using static Marketplace.PaidServices.Domain.Orders.Events;

namespace Marketplace.PaidServices
{
    internal static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<V1.OrderCreated>("OrderCreated");
            Map<V1.ServiceAddedToOrder>("ServiceAddedToOrder");
            Map<V1.ServiceRemovedFromOrder>("ServiceRemovedFromOrder");

            Map<OrderCreated>("PaidClassifiedAdCreated");
            Map<ServiceActivated>("AdServiceActivated");
            Map<ServiceDeactivated>("AdServiceDeactivated");
        }
    }
}