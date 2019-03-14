using System;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.Ads.Events;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class Picture : Entity<PictureId>
    {
        public ClassifiedAdId ParentId { get; private set; }
        public PictureSize Size { get; private set; }
        public Uri Location { get; private set; }
        public int Order { get; private set; }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case PictureAddedToAClassifiedAd e:
                    ParentId = new ClassifiedAdId(e.ClassifiedAdId);
                    Id = new PictureId(e.PictureId);
                    Location = new Uri(e.Url);
                    Size = new PictureSize {Height = e.Height, Width = e.Width};
                    Order = e.Order;
                    break;
                case ClassifiedAdPictureResized e:
                    Size = new PictureSize{Height = e.Height, Width = e.Width};
                    break;
            }
        }
        
        public void Resize(PictureSize newSize)
            => Apply(new ClassifiedAdPictureResized
            {
                PictureId = Id.Value,
                ClassifiedAdId = ParentId.Value,
                Height = newSize.Width,
                Width = newSize.Width
            });

        public Picture(Action<object> applier) : base(applier)
        {
        }
    }

    public class PictureId : Value<PictureId>
    {
        public PictureId(Guid value) => Value = value;

        public Guid Value { get; }
    }
}