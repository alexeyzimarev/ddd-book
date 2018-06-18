using System;

namespace Marketplace.Domain
{
public class ClassifiedAd
{
    public Guid Id { get; }
    
    private UserId _ownerId;

    public ClassifiedAd(Guid id, UserId ownerId)
    {
        if (id == default)
            throw new ArgumentException("Identity must be specified", nameof(id));

        Id = id;
        _ownerId = ownerId;
    }
    
    // rest of the code skipped
        

        
        public void SetTitle(string title) => _title = title;

        public void UpdateText(string text) => _text = text;

        public void UpdatePrice(double price) => _price = price;

        private string _title;
        private string _text;
        private double _price;
    }
}