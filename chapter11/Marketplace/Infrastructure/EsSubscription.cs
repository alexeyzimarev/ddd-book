using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.ClassifiedAd;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Marketplace.Infrastructure
{
    public class EsSubscription
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<EsSubscription>();

        private readonly IEventStoreConnection _connection;
        private readonly IList<ReadModels.ClassifiedAdDetails> _items;
        private EventStoreAllCatchUpSubscription _subscription;

        public EsSubscription(IEventStoreConnection connection, IList<ReadModels.ClassifiedAdDetails> items)
        {
            _connection = connection;
            _items = items;
        }

        public void Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500,
                Log.IsEnabled(LogEventLevel.Verbose),
                true, "try-out-subscription");

            _subscription = _connection.SubscribeToAllFrom(Position.Start,
                settings, EventAppeared);
        }

        private Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return Task.CompletedTask;
            
            var @event = resolvedEvent.Deserialzie();
            
            Log.Debug("Projecting event {type}", @event.GetType().Name);

            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    _items.Add(new ReadModels.ClassifiedAdDetails
                    {
                        ClassifiedAdId = e.Id
                    });
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    UpdateItem(e.Id, ad => ad.Title = e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    UpdateItem(e.Id, ad => ad.Description = e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    UpdateItem(e.Id, ad =>
                    {
                        ad.Price = e.Price;
                        ad.CurrencyCode = e.CurrencyCode;
                    });
                    break;
            }

            return Task.CompletedTask;
        }

        private void UpdateItem(Guid id, Action<ReadModels.ClassifiedAdDetails> update)
        {
            var item = _items.FirstOrDefault(x => x.ClassifiedAdId == id);
            if (item == null) return;

            update(item);
        }

        public void Stop() => _subscription.Stop();
    }
}