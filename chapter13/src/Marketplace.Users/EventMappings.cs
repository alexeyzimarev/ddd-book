using static Marketplace.EventSourcing.TypeMapper;
using static Marketplace.Users.Domain.UserProfiles.Events;

namespace Marketplace.Users
{
    internal static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<V1.UserRegistered>("UserRegistered");
            Map<V1.UserFullNameUpdated>("UserFullNameUpdated");
        }
    }
}