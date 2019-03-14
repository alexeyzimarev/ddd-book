using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.UserProfile
{
    public static class Queries
    {
        public static async Task<T> GetUserDetails<T>(
            this Func<IAsyncDocumentSession> getSession, 
            Guid id, Func<UserDetails, T> getProperty)
        {
            using (var session = getSession())
            {
                var userDetails = 
                    await session.LoadAsync<UserDetails>(id.ToString());
                return userDetails == null 
                    ? default 
                    : getProperty(userDetails);
            }
        }
        
        public static async Task<UserDetails> GetUserDetails(
            this Func<IAsyncDocumentSession> getSession, 
            Guid id)
        {
            using (var session = getSession())
            {
                return await session.LoadAsync<UserDetails>(id.ToString());
            }
        }
    }
}