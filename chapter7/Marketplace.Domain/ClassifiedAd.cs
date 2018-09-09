using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
    {
        private string _databaseId { get; set; }
        
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }
        public List<Picture> Pictures { get; private set; }

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Pictures = new List<Picture>();
            Apply(new Events.ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });
        }

        public void SetTitle(ClassifiedAdTitle title) =>
            Apply(new Events.ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });

        public void UpdateText(ClassifiedAdText text) =>
            Apply(new Events.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });

        public void UpdatePrice(Price price) =>
            Apply(new Events.ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = price.Amount,
                CurrencyCode = price.Currency.CurrencyCode
            });

        public void RequestToPublish() =>
            Apply(new Events.ClassidiedAdSentForReview {Id = Id});
        
        public void AddPicture(Uri pictureUri, PictureSize size) =>
            Apply(new Events.PictureAddedToAClassifiedAd
            {
                PictureId = new Guid(),
                ClassifiedAdId = Id,
                Url = pictureUri.ToString(),
                Height = size.Height,
                Width = size.Width,
                Order = Pictures.Max(x => x.Order)
            });

        public void AddPictureSize(PictureId pictureId, Uri pictureUri, PictureSize size)
        {
            if (Pictures.All(x => x.Id != pictureId))
                throw new InvalidOperationException("Picture with the specified id is not found");
            
            Apply(new Events.PictureSizeAddedToAPicture
            {
                PictureId = pictureId.Value,
                ClassigiedAdId = Id,
                Url = pictureUri.ToString(),
                Height = size.Height,
                Width = size.Width
            });
        }

        protected override void When(object @event)
        {
            Picture picture;
            
            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    Id = new ClassifiedAdId(e.Id);
                    OwnerId = new UserId(e.OwnerId);
                    State = ClassifiedAdState.Inactive;
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    Title = new ClassifiedAdTitle(e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    Text = new ClassifiedAdText(e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    Price = new Price(e.Price, e.CurrencyCode);
                    break;
                case Events.ClassidiedAdSentForReview _:
                    State = ClassifiedAdState.PendingReview;
                    break;
                
                // picture
                case Events.PictureAddedToAClassifiedAd e:
                    picture = new Picture(Apply);
                    ApplyToEntity(picture, e);
                    Pictures.Add(picture);
                    break;
                case Events.PictureSizeAddedToAPicture e:
                    picture = Pictures.FirstOrDefault(x => x.Id == new PictureId(e.PictureId));
                    ApplyToEntity(picture, @event);
                    break;
            }
        }

        protected override void EnsureValidState()
        {
            bool valid = Id != null && OwnerId != null;
            switch (State)
            {
                case ClassifiedAdState.PendingReview:
                    valid = valid
                            && Title != null
                            && Text != null
                            && Price?.Amount > 0;
                    break;
                case ClassifiedAdState.Active:
                    valid = valid
                            && Title != null
                            && Text != null
                            && Price?.Amount > 0
                            && ApprovedBy != null;
                    break;
            }

            if (!valid)
                throw new InvalidEntityStateException(this, $"Post-checks failed in state {State}");
        }

        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }
}