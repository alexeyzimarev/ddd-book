using System;
using System.Threading.Tasks;
using Marketplace.Contracts;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.Api
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private readonly ClassifiedAdsApplicationService _applicationService;
        private static ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApi>();

        public ClassifiedAdsCommandsApi(
            ClassifiedAdsApplicationService applicationService)
            => _applicationService = applicationService;

        [HttpPost]
        public Task<IActionResult> Post(ClassifiedAds.V1.Create request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("name")]
        [HttpPut]
        public Task<IActionResult> Put(ClassifiedAds.V1.SetTitle request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("text")]
        [HttpPut]
        public async Task<IActionResult> Put(ClassifiedAds.V1.UpdateText request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        [Route("price")]
        [HttpPut]
        public async Task<IActionResult> Put(ClassifiedAds.V1.UpdatePrice request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        [Route("publish")]
        [HttpPut]
        public async Task<IActionResult> Put(ClassifiedAds.V1.RequestToPublish request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        private async Task<IActionResult> HandleRequest<T>(T request, Func<T, Task> handler)
        {
            try
            {
                Log.Debug("Handling HTTP request of type {type}", typeof(T).Name);
                await handler(request);
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("Error handling the request", e);
                return new BadRequestObjectResult(new {error = e.Message, stackTrace = e.StackTrace});
            }
        }
    }
}