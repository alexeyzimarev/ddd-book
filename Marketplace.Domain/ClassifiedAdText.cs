using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAdText : Value<ClassifiedAdText>
    {
        public string Value { get; }

        private ClassifiedAdText(string text) => Value = text;
        
        public ClassifiedAdText FromString(string text) =>
            new ClassifiedAdText(text);
    }
}