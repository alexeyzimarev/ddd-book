using System;
using System.Threading.Tasks;
using Marketplace.Ads.Messages.Ads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.FunctionalAd
{
    [Route("api/func"), Authorize]
    public class FunctionalCommandApi : ControllerBase
    {
        readonly FunctionalCommandService _app;
        
        public FunctionalCommandApi(FunctionalCommandService app) => _app = app;

        [HttpPost]
        public Task<ActionResult> Post(Commands.V1.Create command)
            => this.HandleCommand(
                command, 
                cmd => _app.Handle(cmd),
                cmd => cmd.OwnerId = GetUserId());

        [Route("title"), HttpPut]
        public Task<ActionResult> Put(Commands.V1.ChangeTitle command)
            => this.HandleCommand(
                command,
                cmd => _app.Handle(cmd));

        Guid GetUserId() => Guid.Parse(User.Identity.Name);
    }
}