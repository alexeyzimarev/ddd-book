using System;
using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Functional;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using Marketplace.EventStore;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.Ads.Domain.Functional.FunctionalAd;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Modules.FunctionalAd
{
    public class FunctionalCommandService : FunctionalCommandService<FunctionalAdState>
    {
        public FunctionalCommandService(FunctionalStore store) : base(store) { }

        public Task Handle(V1.Create command)
            => Handle(
                command.Id,
                state => Create(
                    new ClassifiedAdId(command.Id),
                    new UserId(command.OwnerId)
                )
            );

        public Task Handle(V1.ChangeTitle command)
            => Handle(
                command.Id,
                state => SetTitle(
                    state,
                    ClassifiedAdTitle.FromString(command.Title)
                )
            );
    }

    public abstract class FunctionalCommandService<T>
        where T : class, IAggregateState<T>, new()
    {
        readonly FunctionalStore _store;

        protected FunctionalCommandService(FunctionalStore store) 
            => _store = store;

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