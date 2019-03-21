using System;
using System.Threading.Tasks;
using Marketplace.Infrastructure.RavenDb;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using static Marketplace.Modules.Projections.ReadModels;

namespace Marketplace.Modules.ClassifiedAds
{
    [ApiController, Route("/ad")]
    public class ClassifiedAdsQueryApi : ControllerBase
    {
        readonly Func<IAsyncDocumentSession> _getSession;

        public ClassifiedAdsQueryApi(Func<IAsyncDocumentSession> getSession) 
            => _getSession = getSession;

        [HttpGet]
        public Task<ActionResult<ClassifiedAdDetails>> Get(
            [FromQuery] QueryModels.GetPublicClassifiedAd request)
            => _getSession.RunApiQuery(s => s.Query(request));
    }
}