using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.Ads.Messages.Ads;

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

        public override TestAdState When(object @event)
        {
            var newState = @event switch
            {
                Events.ClassifiedAdCreated e =>
                    With(
                        x =>
                        {
                            x.Id = e.Id;
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

        protected override bool EnsureValidState(TestAdState newState)
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

        public TestAdState()
        {
            Version = -1;
            State = ClassifiedAd.ClassifiedAdState.Inactive;
        }
    }
}