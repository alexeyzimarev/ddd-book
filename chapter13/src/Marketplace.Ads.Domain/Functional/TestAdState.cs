using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.Ads.Events;

namespace Marketplace.Ads.Domain.Functional
{
    public class TestAdState : AggregateState<TestAdState>
    {
        internal UserId OwnerId { get; set; }
        ClassifiedAdTitle Title { get; set; }
        ClassifiedAdText Text { get; set; }
        Price Price { get; set; }
        ClassifiedAd.ClassifiedAdState State { get; set; }
        UserId ApprovedBy { get; set; }

        public override TestAdState When(TestAdState state, object @event)
        {
            var newState = @event switch
            {
                ClassifiedAdCreated e =>
                    With(state,
                        x =>
                        {
                            x.Id = e.Id;
                            x.OwnerId = new UserId(e.OwnerId);
                        }
                    ),
                ClassifiedAdTitleChanged e =>
                    With(state, x => x.Title = new ClassifiedAdTitle(e.Title)),
                ClassifiedAdTextUpdated e =>
                    With(state, x => x.Text = new ClassifiedAdText(e.AdText)),
                ClassifiedAdPriceUpdated e =>
                    With(state, x => x.Price = new Price(e.Price, e.CurrencyCode)),
                ClassifiedAdSentForReview _ =>
                    With(state,
                        x => x.State = ClassifiedAd.ClassifiedAdState.PendingReview
                    ),
                ClassifiedAdPublished e =>
                    With(state,
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

        protected override bool EnsureValidState(TestAdState newState)
            => newState switch
               {
                   TestAdState ad when ad.OwnerId == null => false,
                   TestAdState ad when (ad.State == ClassifiedAd.ClassifiedAdState.PendingReview || ad.State == ClassifiedAd.ClassifiedAdState.Active)
                                       && ad.Title != null
                                       && ad.Text != null
                                       && ad.Price?.Amount > 0 => false,
                   TestAdState ad when ad.State == ClassifiedAd.ClassifiedAdState.Active
                                       && ad.ApprovedBy != null => false,
                   _ => true
               };

        public TestAdState()
        {
            Version = -1;
            State = ClassifiedAd.ClassifiedAdState.Inactive;
        }
    }
}