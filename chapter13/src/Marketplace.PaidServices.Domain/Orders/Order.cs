using Marketplace.PaidServices.Domain.ClassifiedAds;
using Marketplace.PaidServices.Domain.Services;
using static Marketplace.PaidServices.Messages.Orders.Events;

namespace Marketplace.PaidServices.Domain.Orders
{
    public static class Order
    {
        public static OrderState.Result Create(
            OrderId orderId,
            ClassifiedAdId classifiedAdId
        )
            => new OrderState().Apply(
                new V1.OrderCreated
                {
                    OrderId = orderId,
                    ClassifiedAdId = classifiedAdId
                }
            );

        public static OrderState.Result AddService(
            OrderState state,
            PaidService service
        )
            => state.Apply(
                new V1.ServiceAddedToOrder
                {
                    OrderId = state.Id,
                    ServiceType = service.Type.ToString(),
                    Description = service.Description,
                    Price = service.Price
                },
                new V1.OrderTotalUpdated
                {
                    OrderId = state.Id,
                    Totel = state.GetTotal() + service.Price
                }
            );

        public static OrderState.Result RemoveService(
            OrderState state,
            PaidService service
        )
            => state.Apply(
                new V1.ServiceRemovedFromOrder
                {
                    OrderId = state.Id,
                    ServiceType = service.Type.ToString()
                },
                new V1.OrderTotalUpdated
                {
                    OrderId = state.Id,
                    Totel = state.GetTotal() - service.Price
                }
            );

        public static OrderState.Result Fulfill(OrderState state)
            => state.Apply(
                new V1.OrderFulfilled
                {
                    OrderId = state.Id,
                    ClassifiedAdId = state.AdId,
                    Services = state.GetServices()
                }
            );
    }
}