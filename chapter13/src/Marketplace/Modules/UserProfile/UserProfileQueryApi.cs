using System;
using System.Threading.Tasks;
using Marketplace.Modules.Projections;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace Marketplace.Modules.UserProfile
{
    [Route("api/profile")]
    public class UserProfileQueryApi : ControllerBase
    {
        private readonly Func<IAsyncDocumentSession> _getSession;
        
        public UserProfileQueryApi(Func<IAsyncDocumentSession> getSession) 
            => _getSession = getSession;

        [HttpGet("{userId}")]
        public async Task<ActionResult<ReadModels.UserDetails>> Get(Guid userId)
        {
            var user = await _getSession.GetUserDetails(userId);

            if (user == null) return NotFound();

            return user;
        }
    }
}