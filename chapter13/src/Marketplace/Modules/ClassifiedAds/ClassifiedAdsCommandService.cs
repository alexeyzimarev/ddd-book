using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Modules.ClassifiedAds
{
    public class ClassifiedAdsCommandService :
        ApplicationService<ClassifiedAd, ClassifiedAdId>
    {
        public ClassifiedAdsCommandService(
            IAggregateStore store, ICurrencyLookup currencyLookup) : base(store)
        {
            CreateWhen<V1.Create>(
                cmd => new ClassifiedAdId(cmd.Id),
                (cmd, id) => new ClassifiedAd(
                    id, new UserId(cmd.OwnerId)));
            
            UpdateWhen<V1.ChangeTitle>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title)));
            
            UpdateWhen<V1.UpdateText>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text)));
            
            UpdateWhen<V1.UpdatePrice>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.UpdatePrice(
                    Price.FromDecimal(cmd.Price, cmd.Currency ?? "EUR", currencyLookup)));
            
            UpdateWhen<V1.RequestToPublish>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.RequestToPublish());
            
            UpdateWhen<V1.Publish>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.Publish(new UserId(cmd.ApprovedBy)));
            
            UpdateWhen<V1.Delete>(
                cmd => new ClassifiedAdId(cmd.Id), 
                (ad, cmd) => ad.Delete());
        }
    }
}