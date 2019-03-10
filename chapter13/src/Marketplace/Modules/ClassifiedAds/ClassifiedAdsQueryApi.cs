using System.Threading.Tasks;
using Marketplace.Infrastructure.WebApi;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;

namespace Marketplace.Modules.ClassifiedAds
{
    [ApiController, Route("/ad")]
    public class ClassifiedAdsQueryApi : ControllerBase
    {
        private readonly IAsyncDocumentSession _session;
        private static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsQueryApi>();

        public ClassifiedAdsQueryApi(IAsyncDocumentSession session)
            => _session = session;

        [HttpGet]
        public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request)
            => RequestHandler.HandleQuery(() => _session.Query(request), Log);
    }
}