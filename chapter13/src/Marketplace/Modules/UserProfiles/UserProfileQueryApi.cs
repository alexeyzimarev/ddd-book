using System;
using System.Threading.Tasks;
using Marketplace.Modules.UserProfile;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.UserProfiles
{
    [Route("api/profile")]
    public class UserProfileQueryApi : ControllerBase
    {
        readonly Func<IAsyncDocumentSession> _getSession;

        public UserProfileQueryApi(
            Func<IAsyncDocumentSession> getSession)
            => _getSession = getSession;

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDetails>> Get(Guid userId)
        {
            var user = await _getSession.GetUserDetails(userId);

            if (user == null) return NotFound();

            return user;
        }
    }
}