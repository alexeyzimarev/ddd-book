using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAd : Entity<ClassifiedAdId>
    {
        public ClassifiedAdId Id { get; }

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Id = id;
            OwnerId = ownerId;
            State = ClassifiedAdState.Inactive;
            
            Raise(new Events.ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });
        }

        public void SetTitle(ClassifiedAdTitle title)
        {
            Title = title;
            
            Raise(new Events.ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });
        }

        public void UpdateText(ClassifiedAdText text)
        {
            Text = text;
            
            Raise(new Events.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });
        }

        public void UpdatePrice(Price price)
        {
            Price = price;
            
            Raise(new Events.ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = Price.Amount,
                CurrencyCode = Price.Currency.CurrencyCode
            });
        }

        public void RequestToPublish()
        {
            State = ClassifiedAdState.PendingReview;
            
            Raise(new Events.ClassidiedAdSentForReview{Id = Id});
        }
        
        public UserId OwnerId { get; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }

        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }
}