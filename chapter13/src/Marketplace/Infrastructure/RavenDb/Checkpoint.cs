using EventStore.ClientAPI;

namespace Marketplace.Infrastructure.RavenDb
{
    public class Checkpoint
    {
        public string Id { get; set; }
        public Position Position { get; set; }
    }
}