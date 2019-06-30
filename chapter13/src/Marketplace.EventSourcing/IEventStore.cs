using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface IEventStore
    {
        Task AppendEvents(
            string streamName,
            long version,
            params object[] events
        );

        Task AppendEvents(
            string streamName,
            params object[] events
        );

        Task<IEnumerable<object>> LoadEvents(string stream);
        
        Task<bool> StreamExists(string stream);
    }
}
