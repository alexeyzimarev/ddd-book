using System.Threading.Tasks;
using Marketplace.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.PaidServices.Orders.Commands;

namespace Marketplace.PaidServices.Orders
{
    [Route("api/order"), Authorize]
    public class OrdersCommandApi : ControllerBase
    {
        readonly OrdersCommandService _app;

        public OrdersCommandApi(OrdersCommandService commandService)
            => _app = commandService;

        [HttpPost]
        public Task<ActionResult> Post(V1.CreateOrder command)
            => this.HandleCommand(_app.Handle(command));

        [Route("addservice"), HttpPost]
        public Task<ActionResult> Post(V1.AddService command)
            => this.HandleCommand(_app.Handle(command));

        [Route("removeservice"), HttpPost]
        public Task<ActionResult> Post(V1.RemoveService command)
            => this.HandleCommand(_app.Handle(command));

        [Route("fulfill"), HttpPost]
        public Task<ActionResult> Post(V1.FulfillOrder command)
            => this.HandleCommand(_app.Handle(command));
    }
}