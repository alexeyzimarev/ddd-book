using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.Infrastructure.WebApi
{
    public abstract class BaseController<T, TId> : ControllerBase 
        where T : AggregateRoot<TId>
    {
        private ApplicationService<T, TId> Service { get; }

        private static readonly ILogger Log = 
            Serilog.Log.ForContext<BaseController<T, TId>>();

        protected BaseController(ApplicationService<T, TId> applicationService) 
            => Service = applicationService;

        protected async Task<IActionResult> HandleCommand<TCommand>(
            TCommand command, Action<TCommand> commandModifier = null)
        {
            try
            {
                Log.Debug("Handling HTTP request of type {type}", typeof(T).Name);
                commandModifier?.Invoke(command);
                await Service.Handle(command);
                return new OkResult();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error handling the command");
                return new BadRequestObjectResult(new
                {
                    error = e.Message, stackTrace = e.StackTrace
                });
            }
        }

        protected Guid GetUserId() =>
            Guid.Parse(User.Identity.Name);
    }
}