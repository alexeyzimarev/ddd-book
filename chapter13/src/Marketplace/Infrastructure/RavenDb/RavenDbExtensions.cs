using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure.RavenDb
{
    public static class RavenDbExtensions
    {
        public static async Task<ActionResult<T>> RunApiQuery<T>(
            this Func<IAsyncDocumentSession> getSession,
            Func<IAsyncDocumentSession, Task<T>> query)
        {
            using (var session = getSession())
            {
                try
                {
                    return new OkObjectResult(await query(session));
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(
                        new
                        {
                            error = e.Message,
                            stackTrace = e.StackTrace
                        }
                    );
                }
            }
        }
        
        public static async Task UpdateItem<T>(
            this IAsyncDocumentSession session,
            string id,
            Action<T> update)
        {
            var item = await session.LoadAsync<T>(id);

            if (item == null) return;

            update(item);
        }

        public static async Task UpdateMultipleItems<T>(
            this IAsyncDocumentSession session,
            Expression<Func<T, bool>> query,
            Action<T> update)
        {
            var items = await session
                .Query<T>()
                .Where(query)
                .ToListAsync();

            foreach (var item in items)
                update(item);
        }

    }
}