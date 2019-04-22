using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.UserProfile.Events;

namespace Marketplace.Modules.UserProfiles
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            TypeMapper.Map<V1.UserRegistered>("UserRegistered");
            TypeMapper.Map<V1.UserFullNameUpdated>("UserFullNameUpdated");
        }
    }
}