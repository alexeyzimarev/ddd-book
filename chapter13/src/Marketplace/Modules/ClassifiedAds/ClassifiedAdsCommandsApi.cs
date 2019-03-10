using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.EventSourcing;
using Marketplace.Infrastructure.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.ClassifiedAds
{
    [ApiController, Route("api/ad"), Authorize]
    public class ClassifiedAdsCommandsApi : BaseController<ClassifiedAd, ClassifiedAdId>
    {
        public ClassifiedAdsCommandsApi(
            ApplicationService<ClassifiedAd, ClassifiedAdId> applicationService) 
            : base(applicationService) { }

        [HttpPost]
        public Task<IActionResult> Post(Contracts.V1.Create command)
            => HandleCommand(command, cmd => cmd.OwnerId = GetUserId());

        [Route("name")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.SetTitle command)
            => HandleCommand(command);

        [Route("text")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdateText command)
            => HandleCommand(command);

        [Route("price")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdatePrice command)
            => HandleCommand(command);

        [Route("requestpublish")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.RequestToPublish command)
            => HandleCommand(command);

        [Route("publish")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.Publish command)
            => HandleCommand(command);
    }
}