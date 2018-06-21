namespace Marketplace.Domain
{
    public class ClassifiedAd
    {
        public ClassifiedAdId Id { get; }

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Id = id;
            _ownerId = ownerId;
            _state = ClassifiedAdState.Inactive;
            EnsureValidState();
        }

        public void SetTitle(ClassifiedAdTitle title)
        {
            _title = title;
            EnsureValidState();
        }

        public void UpdateText(ClassifiedAdText text)
        {
            _text = text;
            EnsureValidState();
        }

        public void UpdatePrice(Price price)
        {
            _price = price;
            EnsureValidState();
        }

        public void RequestToPublish()
        {
            _state = ClassifiedAdState.PendingReview;
            EnsureValidState();
        }

        private void EnsureValidState()
        {
            bool valid = Id != null && _ownerId != null;
            switch (_state)
            {
                case ClassifiedAdState.PendingReview:
                    valid = valid 
                            && _title != null 
                            && _text != null 
                            && _price.Amount > 0;
                    break;
                case ClassifiedAdState.Active:
                    valid = valid 
                            && _title != null 
                            && _text != null 
                            && _price.Amount > 0
                            && _approvedBy != null;
                    break;
            }

            if (!valid)
                throw new InvalidEntityStateException(this, $"Post-checks failed in state {_state}");
        }

        private UserId _ownerId;
        private ClassifiedAdTitle _title;
        private ClassifiedAdText _text;
        private Price _price;
        private ClassifiedAdState _state;
        private UserId _approvedBy;

        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }
}