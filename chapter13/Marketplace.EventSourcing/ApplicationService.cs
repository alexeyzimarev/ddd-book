using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.EventSourcing.Logging;

namespace Marketplace.EventSourcing
{
    public class ApplicationService<T, TId> where T : AggregateRoot<TId>
    {
        private readonly IAggregateStore _store;
        private static ILog _log => LogProvider.For<ApplicationService<T, TId>>();

        protected ApplicationService(IAggregateStore store)
            => _store = store;

        private readonly Dictionary<Type, Func<object, Task>> _handlers =
            new Dictionary<Type, Func<object, Task>>();

        public Task Handle<TCommand>(TCommand command)
        {
            if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
                throw new InvalidOperationException($"No registered handler for command {typeof(TCommand).Name}");
            return handler(command);
        }

        protected void CreateWhen<TCommand>(
            Func<TCommand, TId> getAggregateId,
            Func<TCommand, TId, T> creator) where TCommand : class
            => When<TCommand>(
                async command =>
                {
                    var aggregateId = getAggregateId(command);
                    if (await _store.Exists<T, TId>(aggregateId))
                        throw new InvalidOperationException($"Entity with id {aggregateId.ToString()} already exists");

                    var aggregate = creator(command, aggregateId);

                    await _store.Save<T, TId>(aggregate);
                });

        protected void UpdateWhen<TCommand>(
            Func<TCommand, TId> getAggregateId,
            Action<T, TCommand> updater) where TCommand : class
            => When<TCommand>(
                async command =>
                {
                    var aggregateId = getAggregateId(command);
                    var aggregate = await _store.Load<T, TId>(aggregateId);
                    if (aggregate == null)
                        throw new InvalidOperationException($"Entity with id {aggregateId.ToString()} cannot be found");

                    updater(aggregate, command);
                    await _store.Save<T, TId>(aggregate);
                });

        private void When<TCommand>(Func<TCommand, Task> handler) where TCommand : class
            => _handlers.Add(typeof(TCommand), c => handler((TCommand) c));
    }
}