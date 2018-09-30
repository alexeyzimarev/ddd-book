using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.ClassifiedAd
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
        public Task<IActionResult> Post(Contracts.V1.Create request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("name")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.SetTitle request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("text")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdateText request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("price")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.UpdatePrice request)
            => HandleRequest(request, _applicationService.Handle);

        [Route("publish")]
        [HttpPut]
        public Task<IActionResult> Put(Contracts.V1.RequestToPublish request)
            => HandleRequest(request, _applicationService.Handle);

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