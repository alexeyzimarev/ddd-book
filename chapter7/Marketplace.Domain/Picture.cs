using System;
using System.Collections.Generic;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Picture : Entity<PictureId>
    {
        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureAddedToAClassifiedAd e:
                    Id = new PictureId(e.PictureId);
                    _images.Add(new ImageDetails(
                        new PictureSize(e.Height, e.Width),
                        new Uri(e.Url),
                        0));
                    break;
            }
        }

        internal int Order { get; private set; }
        private readonly List<ImageDetails> _images;

        public Picture(Action<object> applier) : base(applier)
        {
            _images = new List<ImageDetails>();
        }

        public void AddResizedImage()
        {
            
        }

        private struct ImageDetails
        {
            public ImageDetails(PictureSize size, Uri location, int order)
            {
                Size = size;
                Location = location;
                Order = order;
            }

            internal PictureSize Size { get; set; }
            internal Uri Location { get; set; }
            internal int Order { get; set; }
        }
    }

    public class PictureId : Value<PictureId>
    {
        public PictureId(Guid value) => Value = value;

        public Guid Value { get; }
    }
}