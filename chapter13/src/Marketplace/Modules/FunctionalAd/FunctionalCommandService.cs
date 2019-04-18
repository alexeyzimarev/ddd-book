using System;
using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Functional;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;
using Marketplace.EventSourcing;
using Marketplace.Infrastructure.EventStore;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.FunctionalAd
{
    public class FunctionalCommandService : FunctionalCommandService<FunctionalAdState>
    {
        public FunctionalCommandService(FunctionalStore store) : base(store) { }

        public Task Handle(Commands.V1.Create command)
            => Handle(
                command.Id,
                state => Ads.Domain.Functional.FunctionalAd.Create(
                    new ClassifiedAdId(command.Id),
                    new UserId(command.OwnerId)
                )
            );

        public Task Handle(Commands.V1.ChangeTitle command)
            => Handle(
                command.Id,
                state => Ads.Domain.Functional.FunctionalAd.SetTitle(
                    state,
                    ClassifiedAdTitle.FromString(command.Title)
                )
            );
    }

    public abstract class FunctionalCommandService<T>
        where T : class, IAggregateState<T>, new()
    {
        readonly FunctionalStore _store;

        protected FunctionalCommandService(FunctionalStore store) => _store = store;

        Task<T> Load(Guid id)
            => _store.Load<T>(
                id,
                (x, e) => x.When(x, e)
            );

        protected async Task Handle(
            Guid id,
            Func<T, AggregateState<T>.Result> update)
        {
            var state = await Load(id);
            await _store.Save(state.Version, update(state));
        }
    }

    public static class ControllerExtensions
    {
        public static async Task<ActionResult> HandleCommand<TCommand>(
            this ControllerBase _,
            TCommand command,
            Func<TCommand, Task> handler,
            Action<TCommand> commandModifier = null)
        {
            try
            {
                commandModifier?.Invoke(command);
                await handler(command);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error = e.Message, 
                        stackTrace = e.StackTrace
                    }
                );
            }
            
        }
    }
}