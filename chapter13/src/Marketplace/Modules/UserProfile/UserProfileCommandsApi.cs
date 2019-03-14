using System.Threading.Tasks;
using Marketplace.Infrastructure.WebApi;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static Marketplace.Ads.Messages.UserProfile.Commands;

namespace Marketplace.Modules.UserProfile
{
    [ApiController, Route("api/profile")]
    public class UserProfileCommandsApi : ControllerBase
    {
        private readonly UserProfileCommandService _applicationService;
        private static readonly ILogger Log = Serilog.Log.ForContext<UserProfileCommandsApi>();

        public UserProfileCommandsApi(UserProfileCommandService applicationService) 
            => _applicationService = applicationService;

        [HttpPost]
        public Task<IActionResult> Post(V1.RegisterUser request)
            => RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        
        [Route("fullname")]
        [HttpPut]
        public Task<IActionResult> Put(V1.UpdateUserFullName request)
            => RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        
        [Route("displayname")]
        [HttpPut]
        public Task<IActionResult> Put(V1.UpdateUserDisplayName request)
            => RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
        
        [Route("photo")]
        [HttpPut]
        public Task<IActionResult> Put(V1.UpdateUserProfilePhoto request)
            => RequestHandler.HandleCommand(request, _applicationService.Handle, Log);
    }
}