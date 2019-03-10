using System;
using System.Threading.Tasks;
using Marketplace.Modules.UserProfile;
using Raven.Client.Documents.Session;

namespace Marketplace.Modules.Auth
{
    public class AuthService
    {
        private readonly Func<IAsyncDocumentSession> _getSession;

        public AuthService(Func<IAsyncDocumentSession> getSession) 
            => _getSession = getSession;

        public async Task<bool> CheckCredentials(string userName, string password)
        {
            var userDetails = await _getSession.GetUserDetails(Guid.Parse(password));
            
            return userDetails != null && userDetails.DisplayName == userName;
        }
    }
}