using System;

namespace Marketplace.Domain
{
    public class ClassifiedAdId : IEquatable<ClassifiedAdId>
    {
        private readonly Guid _value;

        public ClassifiedAdId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "Classified Ad id cannot be empty");
            
            _value = value;
        }

        public static implicit operator Guid(ClassifiedAdId self) => self._value;
        
        public static implicit operator ClassifiedAdId(string value) 
            => new ClassifiedAdId(Guid.Parse(value));

        public override string ToString() => _value.ToString();

        public bool Equals(ClassifiedAdId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassifiedAdId) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}