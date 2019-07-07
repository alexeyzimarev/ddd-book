using Marketplace.PaidServices.Integration.AdsIntegration;
using static Marketplace.EventSourcing.TypeMapper;
using static Marketplace.PaidServices.Integration.ClassifiedAds.Events;

namespace Marketplace.PaidServices.Integration
{
    internal static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<Events.V1.AdPublished>("AdPublished");

            Map<V1.EnrichedAdPublished>(
                "EnrichedAdPublished"
            );
        }
    }
}
