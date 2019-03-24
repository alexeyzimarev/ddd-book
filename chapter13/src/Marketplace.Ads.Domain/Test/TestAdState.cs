using System;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;

namespace Marketplace.Ads.Domain.Test
{
    public interface IAggregateState<out T>
    {
        T When(object @event);

        string GetStreamName(Guid id);

        string StreamName { get; }

        long Version { get; }
    }

    public class TestAdState : IAggregateState<TestAdState>
    {
        public ClassifiedAdId Id { get; private set; }
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAd.ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }

        TestAdState With(Action<TestAdState> update)
            => new TestAdState(this, update);

        public TestAdState When(object @event)
        {
            var newState = @event switch
            {
                Events.ClassifiedAdCreated e =>
                    With(
                        x =>
                        {
                            x.Id = new ClassifiedAdId(e.Id);
                            x.OwnerId = new UserId(e.OwnerId);
                        }
                    ),
                Events.ClassifiedAdTitleChanged e =>
                    With(x => x.Title = new ClassifiedAdTitle(e.Title)),
                Events.ClassifiedAdTextUpdated e =>
                    With(x => x.Text = new ClassifiedAdText(e.AdText)),
                Events.ClassifiedAdPriceUpdated e =>
                    With(x => x.Price = new Price(e.Price, e.CurrencyCode)),
                Events.ClassifiedAdSentForReview _ =>
                    With(
                        x => x.State = ClassifiedAd.ClassifiedAdState.PendingReview
                    ),
                Events.ClassifiedAdPublished e =>
                    With(
                        x =>
                        {
                            x.ApprovedBy = new UserId(e.ApprovedBy);
                            x.State = ClassifiedAd.ClassifiedAdState.Active;
                        }
                    ),
                _ => this
            };
            newState.Version++;
            return newState;
        }

        static bool EnsureValidState(TestAdState newState)
            => newState.OwnerId != null &&
               (newState.State switch
               {
                   ClassifiedAd.ClassifiedAdState.PendingReview =>
                       newState.Title != null
                       && newState.Text != null
                       && newState.Price?.Amount > 0,
                   ClassifiedAd.ClassifiedAdState.Active =>
                       newState.Title != null
                       && newState.Text != null
                       && newState.Price?.Amount > 0
                       && newState.ApprovedBy != null,
                   _ => true
               });

        internal TestAdState Apply(object @event)
        {
            var newState = When(@event);

            if (!EnsureValidState(newState))
                throw new DomainExceptions.InvalidEntityState(
                    this, $"Post-checks failed in state {State}"
                );

            return newState;
        }

        public long Version { get; private set; }

        public string GetStreamName(Guid id) => $"ClassifiedAd-{id}";

        public string StreamName => GetStreamName(Id);

        public TestAdState()
        {
            Version = -1;
            State = ClassifiedAd.ClassifiedAdState.Inactive;
        }

        TestAdState(TestAdState state, Action<TestAdState> update)
        {
            Id = NullOr(state.Id, () => new ClassifiedAdId(state.Id));
            OwnerId = NullOr(state.OwnerId, () => new UserId(state.OwnerId));

            Title = NullOr(
                state.Title, () => new ClassifiedAdTitle(state.Title)
            );
            Text = NullOr(state.Text, () => new ClassifiedAdText(state.Text));

            Price = NullOr(
                state.Price,
                () => new Price(
                    state.Price.Amount, state.Price.Currency.CurrencyCode
                )
            );
            State = state.State;

            ApprovedBy = NullOr(
                state.ApprovedBy, () => new UserId(state.ApprovedBy)
            );
            Version = state.Version;

            update(this);

            T NullOr<T>(T value, Func<T> create) where T : class
                => value == null ? null : create();
        }
    }
}