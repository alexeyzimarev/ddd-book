using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Domain.ClassifiedAds.ClassifiedAd.ClassifiedAdState;

namespace Marketplace.Ads.Domain.Test
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
                Events.ClassifiedAdCreated e =>
                    With(state,
                        x =>
                        {
                            x.Id = e.Id;
                            x.OwnerId = new UserId(e.OwnerId);
                        }
                    ),
                Events.ClassifiedAdTitleChanged e =>
                    With(state, x => x.Title = new ClassifiedAdTitle(e.Title)),
                Events.ClassifiedAdTextUpdated e =>
                    With(state, x => x.Text = new ClassifiedAdText(e.AdText)),
                Events.ClassifiedAdPriceUpdated e =>
                    With(state, x => x.Price = new Price(e.Price, e.CurrencyCode)),
                Events.ClassifiedAdSentForReview _ =>
                    With(state,
                        x => x.State = PendingReview
                    ),
                Events.ClassifiedAdPublished e =>
                    With(state,
                        x =>
                        {
                            x.ApprovedBy = new UserId(e.ApprovedBy);
                            x.State = Active;
                        }
                    ),
                _ => this
            };
            newState.Version++;
            return newState;
        }

        protected override bool EnsureValidState(TestAdState newState)
            => newState.OwnerId != null &&
               (newState switch
               {
                   TestAdState ad when ad.OwnerId == null => false,
                   TestAdState ad when ad.State == PendingReview
                                       && ad.Title != null
                                       && ad.Text != null
                                       && ad.Price?.Amount > 0 => false,
                   TestAdState ad when ad.State == Active
                                       && ad.Title != null
                                       && ad.Text != null
                                       && ad.Price?.Amount > 0
                                       && ad.ApprovedBy != null => false,
                   _ => true
               });

        public TestAdState()
        {
            Version = -1;
            State = Inactive;
        }
    }
}