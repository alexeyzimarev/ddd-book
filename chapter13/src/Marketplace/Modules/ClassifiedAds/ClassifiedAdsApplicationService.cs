using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;

namespace Marketplace.Modules.ClassifiedAds
{
    public class ClassifiedAdsApplicationService :
        ApplicationService<ClassifiedAd, ClassifiedAdId>
    {
        public ClassifiedAdsApplicationService(
            IAggregateStore store, ICurrencyLookup currencyLookup) : base(store)
        {
            CreateWhen<Contracts.V1.Create>(
                cmd => new ClassifiedAdId(cmd.Id),
                (cmd, id) => new ClassifiedAd(
                    id, new UserId(cmd.OwnerId)));
            
            UpdateWhen<Contracts.V1.SetTitle>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
            
            UpdateWhen<Contracts.V1.UpdateText>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
            
            UpdateWhen<Contracts.V1.UpdatePrice>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.UpdatePrice(
                    Price.FromDecimal(cmd.Price, cmd.Currency, currencyLookup)));
            
            UpdateWhen<Contracts.V1.RequestToPublish>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.RequestToPublish());
            
            UpdateWhen<Contracts.V1.Publish>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.Publish(new UserId(cmd.ApprovedBy)));
        }
    }
}