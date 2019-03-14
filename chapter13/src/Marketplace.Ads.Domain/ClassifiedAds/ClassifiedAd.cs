using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.Ads.Events;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
    {
        // Aggregate state properties
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }
        public IEnumerable<Picture> Pictures => _pictures;

        private List<Picture> _pictures;

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Apply(new ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });
        }

        public void SetTitle(ClassifiedAdTitle title) =>
            Apply(new ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });

        public void UpdateText(ClassifiedAdText text) =>
            Apply(new ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });

        public void UpdatePrice(Price price) =>
            Apply(new ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = price.Amount,
                CurrencyCode = price.Currency.CurrencyCode
            });

        public void RequestToPublish() =>
            Apply(new ClassidiedAdSentForReview {Id = Id});

        public void Publish(UserId userId) =>
            Apply(new ClassifiedAdPublished
            {
                Id = Id,
                ApprovedBy = userId,
                OwnerId = OwnerId
            });

        public void Delete() =>
            Apply(new ClassifiedAdDeleted {Id = Id});

        public void AddPicture(Uri pictureUri, PictureSize size) =>
            Apply(new PictureAddedToAClassifiedAd
            {
                PictureId = new Guid(),
                ClassifiedAdId = Id,
                Url = pictureUri.ToString(),
                Height = size.Height,
                Width = size.Width,
                Order = Pictures.Max(x => x.Order)
            });

        public void ResizePicture(PictureId pictureId, PictureSize newSize)
        {
            var picture = FindPicture(pictureId);
            if (picture == null)
                throw new InvalidOperationException("Cannot resize a picture that I don't have");

            picture.Resize(newSize);
        }

        protected override void When(object @event)
        {
            Picture picture;

            switch (@event)
            {
                case ClassifiedAdCreated e:
                    Id = new ClassifiedAdId(e.Id);
                    OwnerId = new UserId(e.OwnerId);
                    State = ClassifiedAdState.Inactive;
                    _pictures = new List<Picture>();
                    break;
                case ClassifiedAdTitleChanged e:
                    Title = new ClassifiedAdTitle(e.Title);
                    break;
                case ClassifiedAdTextUpdated e:
                    Text = new ClassifiedAdText(e.AdText);
                    break;
                case ClassifiedAdPriceUpdated e:
                    Price = new Price(e.Price, e.CurrencyCode);
                    break;
                case ClassidiedAdSentForReview _:
                    State = ClassifiedAdState.PendingReview;
                    break;
                case ClassifiedAdPublished e:
                    ApprovedBy = new UserId(e.ApprovedBy);
                    State = ClassifiedAdState.Active;
                    break;

                // picture
                case PictureAddedToAClassifiedAd e:
                    picture = new Picture(Apply);
                    ApplyToEntity(picture, e);
                    _pictures.Add(picture);
                    break;
                case ClassifiedAdPictureResized e:
                    picture = FindPicture(new PictureId(e.PictureId));
                    ApplyToEntity(picture, @event);
                    break;
            }
        }

        private Picture FindPicture(PictureId id)
            => Pictures.FirstOrDefault(x => x.Id == id);

        private Picture FirstPicture => _pictures.OrderBy(x => x.Order).FirstOrDefault();

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
//                            && FirstPicture.HasCorrectSize();
                    break;
                case ClassifiedAdState.Active:
                    valid = valid
                            && Title != null
                            && Text != null
                            && Price?.Amount > 0
//                            && FirstPicture.HasCorrectSize()
                            && ApprovedBy != null;
                    break;
            }

            if (!valid)
                throw new DomainExceptions.InvalidEntityState(this, $"Post-checks failed in state {State}");
        }

        protected ClassifiedAd()
        {
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