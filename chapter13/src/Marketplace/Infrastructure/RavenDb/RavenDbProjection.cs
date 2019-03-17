using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.Modules.Projections;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Serilog;

namespace Marketplace.Infrastructure.RavenDb
{
    public abstract class RavenDbProjection<T> : IProjection
    {
        readonly ILogger _log;
        static readonly string ReadModelName = typeof(T).Name;

        protected RavenDbProjection(
            Func<IAsyncDocumentSession> getSession,
            Projector projector)
        {
            _projector = projector;
            GetSession = getSession;
            _log = Log.ForContext(GetType());
        }

        Func<IAsyncDocumentSession> GetSession { get; }
        readonly Projector _projector;

        public async Task Project(object @event)
        {
            using (var session = GetSession())
            {
                var handler = _projector(session, @event);

                if (handler != null)
                {
                    _log.Debug(
                        "Projecting {event} to {model}",
                        @event,
                        ReadModelName
                    );
                    
                    await handler();
                    await session.SaveChangesAsync();
                }
            }
        }

        protected static async Task UpdateItem(
            IAsyncDocumentSession session,
            string id,
            Action<T> update)
        {
            var item = await session.LoadAsync<T>(id);

            if (item == null) return;

            update(item);
        }

        protected static async Task UpdateMultipleItems(
            IAsyncDocumentSession session,
            Expression<Func<T, bool>> query,
            Action<T> update)
        {
            var items = await session.Query<T>().Where(query).ToListAsync();

            foreach (var item in items)
                update(item);
        }

        protected delegate Func<Task> Projector(
            IAsyncDocumentSession session,
            object @event);
    }
}