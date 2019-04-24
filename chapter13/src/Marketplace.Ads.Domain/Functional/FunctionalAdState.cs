using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Domain.ClassifiedAds.ClassifiedAd;
using static Marketplace.Ads.Messages.Ads.Events;

namespace Marketplace.Ads.Domain.Functional
{
    public class FunctionalAdState : AggregateState<FunctionalAdState>
    {
        internal UserId OwnerId { get; set; }
        ClassifiedAdTitle Title { get; set; }
        ClassifiedAdText Text { get; set; }
        Price Price { get; set; }
        ClassifiedAdState State { get; set; }
        UserId ApprovedBy { get; set; }

        public override FunctionalAdState When(
            FunctionalAdState state, 
            object @event
        ) => 
            With(@event switch
            {
                V1.ClassifiedAdCreated e =>
                    With(state,
                        x =>
                        {
                            x.Id = e.Id;
                            x.OwnerId = UserId.FromGuid(e.OwnerId);
                        }
                    ),
                V1.ClassifiedAdTitleChanged e =>
                    With(state, x => x.Title = new ClassifiedAdTitle(e.Title)),
                V1.ClassifiedAdTextUpdated e =>
                    With(state, x => x.Text = new ClassifiedAdText(e.AdText)),
                V1.ClassifiedAdPriceUpdated e =>
                    With(state, x => x.Price = new Price(e.Price, e.CurrencyCode)),
                V1.ClassifiedAdSentForReview _ =>
                    With(state,
                        x => x.State = ClassifiedAdState.PendingReview
                    ),
                V1.ClassifiedAdPublished e =>
                    With(state,
                        x =>
                        {
                            x.ApprovedBy = UserId.FromGuid(e.ApprovedBy);
                            x.State = ClassifiedAdState.Active;
                        }
                    ),
                _ => this
            }, x => x.Version++);

        protected override bool EnsureValidState(FunctionalAdState newState)
            => newState switch
               { 
                   { } ad when ad.OwnerId == null => false, 
                   { } ad when (ad.State == ClassifiedAdState.PendingReview 
                                || ad.State == ClassifiedAdState.Active)
                               && ad.Title != null
                               && ad.Text != null
                               && ad.Price?.Amount > 0 => false, 
                    { } ad when ad.State == ClassifiedAdState.Active
                               && ad.ApprovedBy != null => false,
                   _ => true
               };

        public FunctionalAdState()
        {
            Version = -1;
            State = ClassifiedAdState.Inactive;
        }
    }
}