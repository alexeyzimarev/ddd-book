using static Marketplace.Ads.Messages.UserProfile.Events;
using static Marketplace.Infrastructure.EventStore.TypeMapper;

namespace Marketplace.Modules.UserProfile
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<UserRegistered>("UserRegistered");
            Map<UserFullNameUpdated>("UserFullNameUpdated");
        }
    }
}