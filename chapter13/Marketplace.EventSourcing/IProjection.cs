using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface IProjection
    {
        Task Project(object @event);
    }
}