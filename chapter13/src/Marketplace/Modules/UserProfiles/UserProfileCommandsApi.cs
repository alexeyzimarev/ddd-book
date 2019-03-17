using System.Threading.Tasks;
using Marketplace.Ads.Messages.UserProfile;
using Marketplace.EventSourcing;
using Marketplace.Infrastructure.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.UserProfiles
{
    [Route("api/profile")]
    public class UserProfileCommandsApi
        : BaseController<Ads.Domain.UserProfiles.UserProfile>
    {
        [HttpPost]
        public Task<IActionResult> Post(Commands.V1.RegisterUser request)
            => HandleCommand(request);

        [Route("fullname"), HttpPut]
        public Task<IActionResult> Put(Commands.V1.UpdateUserFullName request)
            => HandleCommand(request);

        [Route("displayname"), HttpPut]
        public Task<IActionResult> Put(
            Commands.V1.UpdateUserDisplayName request)
            => HandleCommand(request);

        [Route("photo"), HttpPut]
        public Task<IActionResult> Put(
            Commands.V1.UpdateUserProfilePhoto request)
            => HandleCommand(request);

        public UserProfileCommandsApi(
            ApplicationService<Ads.Domain.UserProfiles.UserProfile>
                applicationService) : base(applicationService) { }
    }
}