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
        static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsQueryApi>();
        readonly IAsyncDocumentSession _session;

        public ClassifiedAdsQueryApi(IAsyncDocumentSession session) => _session = session;

        [HttpGet]
        public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request) => RequestHandler.HandleQuery(() => _session.Query(request), Log);
    }
}