using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        /// <summary>
        ///     Create a new classified ad
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /ad
        ///     {
        ///        "id": "de91f219-2920-423f-a943-1021a7bbac4a",
        ///        "ownerId": "dcec37b0-2536-42d9-9de2-1169c13f5590",
        ///     }
        ///
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post(Contracts.ClassifiedAds.V1.Create request)
        {
            // handle the request here

            return Ok();
        }
    }
}