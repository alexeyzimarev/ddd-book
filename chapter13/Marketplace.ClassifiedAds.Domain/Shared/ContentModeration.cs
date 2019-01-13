using System.Threading.Tasks;

namespace Marketplace.ClassifiedAds.Domain.Shared
{
    public delegate Task<bool> CheckTextForProfanity(string text);
}