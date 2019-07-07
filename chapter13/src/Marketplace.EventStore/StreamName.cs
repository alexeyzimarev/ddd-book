using System;

namespace Marketplace.EventStore
{
    public class StreamName
    {
        string Value { get; }
        const string AllStreamName = "$all";

        StreamName(string value) => Value = value;

        public static StreamName AllStream => new StreamName(AllStreamName);

        public static StreamName For<T>(Guid id)
            => new StreamName($"{typeof(T).Name}-{id:N}");

        public static StreamName Custom(string streamName)
            => new StreamName(streamName);

        public bool IsAllStream => Value.Equals(AllStreamName);

        public override string ToString() => Value ?? "";

        public static implicit operator string(StreamName self)
            => self.ToString();
    }
}
