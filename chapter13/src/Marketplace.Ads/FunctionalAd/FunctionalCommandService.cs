using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.Ads.Domain.Functional;
using Marketplace.Ads.Domain.Shared;
using Marketplace.EventSourcing;
using static Marketplace.Ads.Domain.Functional.FunctionalAd;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Ads.FunctionalAd
{
    public class AdCommandService : CommandService<FunctionalAdState>
    {
        public AdCommandService(IFunctionalAggregateStore store) 
            : base(store) { }

        public Task Handle(V1.Create command)
            => Handle(
                command.Id,
                state => Create(
                    ClassifiedAdId.FromGuid(command.Id),
                    UserId.FromGuid(command.OwnerId)
                )
            );

        public Task Handle(V1.ChangeTitle command)
            => Handle(
                command.Id,
                state => SetTitle(
                    state,
                    ClassifiedAdTitle.FromString(command.Title)
                )
            );
    }
}