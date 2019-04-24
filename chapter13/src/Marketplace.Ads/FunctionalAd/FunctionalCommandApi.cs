using System;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Ads.FunctionalAd
{
    [Route("api/func"), Authorize]
    public class FunctionalCommandApi : ControllerBase
    {
        readonly AdCommandService _app;

        public FunctionalCommandApi(AdCommandService app) => _app = app;

        [HttpPost]
        public Task<ActionResult> Post(V1.Create command)
            => this.HandleCommand(
                _app.Handle(command.With(x => x.OwnerId = GetUserId()))
            );

        [Route("title"), HttpPost]
        public Task<ActionResult> Put(V1.ChangeTitle command)
            => this.HandleCommand(_app.Handle(command));

        Guid GetUserId() => Guid.Parse(User.Identity.Name);
    }
}