using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Modules.Test
{
    [Route("api/test"), Authorize]
    public class TestCommandApi : ControllerBase
    {
        readonly TestAdCommandService _app;
        
        public TestCommandApi(TestAdCommandService app) => _app = app;

        [HttpPost]
        public Task<ActionResult> Post(V1.Create command)
            => this.HandleCommand(
                command, 
                cmd => _app.Handle(cmd),
                cmd => cmd.OwnerId = GetUserId());

        [Route("title"), HttpPut]
        public Task<ActionResult> Put(V1.ChangeTitle command)
            => this.HandleCommand(
                command,
                cmd => _app.Handle(cmd));

        Guid GetUserId() => Guid.Parse(User.Identity.Name);
    }
}