using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.PaidServices.Domain.Services.PaidService;
using static Marketplace.PaidServices.PaidServices.Models;

namespace Marketplace.PaidServices.PaidServices
{
    [Route("/api/services")]
    public class PaidServicesQueryApi : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PaidServiceItem>> Get()
            => Ok(AvailableServices.Select(PaidServiceItem.FromDomain));
    }
}