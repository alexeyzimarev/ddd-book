using System;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.Infrastructure.WebApi
{
    public abstract class BaseController<T> : ControllerBase
        where T : AggregateRoot
    {
        readonly ILogger _log;

        protected BaseController(ApplicationService<T> applicationService)
        {
            _log = Log.ForContext(GetType());
            Service = applicationService;
        }

        ApplicationService<T> Service { get; }

        protected async Task<IActionResult> HandleCommand<TCommand>(
            TCommand command,
            Action<TCommand> commandModifier = null)
        {
            try
            {
                _log.Debug("Handling HTTP request of type {type}", typeof(T).Name);
                commandModifier?.Invoke(command);
                await Service.Handle(command);
                return new OkResult();
            }
            catch (Exception e)
            {
                _log.Error(e, "Error handling the command");

                return new BadRequestObjectResult(
                    new
                    {
                        error = e.Message, stackTrace = e.StackTrace
                    }
                );
            }
        }

        protected Guid GetUserId() => Guid.Parse(User.Identity.Name);
    }
}