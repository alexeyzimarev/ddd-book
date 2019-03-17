using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Infrastructure.EventStore;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure.RavenDb
{
    public class RavenDbCheckpointStore : ICheckpointStore
    {
        readonly string _checkpointName;
        readonly Func<IAsyncDocumentSession> _getSession;

        public RavenDbCheckpointStore(
            Func<IAsyncDocumentSession> getSession,
            string checkpointName)
        {
            _getSession = getSession;
            _checkpointName = checkpointName;
        }

        public async Task<Position?> GetCheckpoint()
        {
            using (var session = _getSession())
            {
                var checkpoint = await session.LoadAsync<Checkpoint>(_checkpointName);
                return checkpoint?.Position ?? AllCheckpoint.AllStart;
            }
        }

        public async Task StoreCheckpoint(Position? position)
        {
            using (var session = _getSession())
            {
                var checkpoint = await session.LoadAsync<Checkpoint>(_checkpointName);

                if (checkpoint == null)
                {
                    checkpoint = new Checkpoint
                    {
                        Id = _checkpointName
                    };
                    await session.StoreAsync(checkpoint);
                }

                checkpoint.Position = position;
                await session.SaveChangesAsync();
            }
        }

        class Checkpoint
        {
            public string Id { get; set; }
            public Position? Position { get; set; }
        }
    }
}