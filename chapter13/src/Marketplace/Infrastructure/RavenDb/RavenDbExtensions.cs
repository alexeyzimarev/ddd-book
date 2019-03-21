using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}