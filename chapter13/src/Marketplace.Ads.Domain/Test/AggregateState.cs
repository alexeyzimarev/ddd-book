using System;
using Force.DeepCloner;
using Marketplace.Ads.Domain.Shared;

namespace Marketplace.Ads.Domain.Test
{
    public interface IAggregateState<out T>
    {
        T When(object @event);

        string GetStreamName(Guid id);

        string StreamName { get; }

        long Version { get; }
    }

    public abstract class AggregateState<T> : IAggregateState<T>
        where T : class, new()
    {
        public abstract T When(object @event);

        public string GetStreamName(Guid id) => $"{typeof(T).Name}-{id:N}";

        public string StreamName => GetStreamName(Id);

        public long Version { get; protected set; }

        protected internal Guid Id { get; set; }

        protected T With(Action<T> update)
        {
            var newState = (this as T).DeepClone();
            update(newState);
            return newState;
        }

        protected abstract bool EnsureValidState(T newState);
        
        internal T Apply(object @event)
        {
            var newState = When(@event);

            if (!EnsureValidState(newState))
                throw new DomainExceptions.InvalidEntityState(
                    this, "Post-checks failed"
                );

            return newState;
        }
    }
}