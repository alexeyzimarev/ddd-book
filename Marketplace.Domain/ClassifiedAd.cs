using System;

namespace Marketplace.Domain
{
    public class ClassifiedAd
    {
        public Guid Id { get; }

        public ClassifiedAd(Guid id)
        {
            if (id == default)
                throw new ArgumentException(nameof(id), "Identity must be specified");
            
            Id = id;
        }

        private Guid _ownerId;
        private string _title;
        private string _text;
        private double _price;
    }
}



