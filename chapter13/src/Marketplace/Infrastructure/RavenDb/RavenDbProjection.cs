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
    public class RavenDbProjection<T> : IProjection
    {
        readonly ILogger _log;
        static readonly string ReadModelName = typeof(T).Name;

        public RavenDbProjection(
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

        public delegate Func<Task> Projector(
            IAsyncDocumentSession session,
            object @event);
    }
}