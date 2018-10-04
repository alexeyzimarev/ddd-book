using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;

namespace Marketplace.ClassifiedAd
{
    [Route("/ad")]
    public class ClassifiedAdsQueryApi : Controller
    {
        private readonly IAsyncDocumentSession _session;

        public ClassifiedAdsQueryApi(IAsyncDocumentSession session)
            => _session = session;

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> Get(QueryModels.GetPublishedClassifiedAds request)
        {
            try
            {
                var ads = await _session.Query(request);
                return Ok(ads);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error handling the query");
                throw;
            }
        }

//        [HttpGet]
//        [Route("myads")]
//        public Task<ActionResult<IEnumerable<ReadModels.PublicClassifiedAdListItem>>>
//            Get(QueryModels.GetOwnersClassifiedAd request)
//        {
//        }
//
//        [HttpGet]
//        [ProducesResponseType((int) HttpStatusCode.OK)]
//        [ProducesResponseType((int) HttpStatusCode.NotFound)]
//        public Task<ActionResult<ReadModels.ClassifiedAdDetails>>
//            Get(QueryModels.GetPublicClassifiedAd request)
//        {
//        }
    }
}