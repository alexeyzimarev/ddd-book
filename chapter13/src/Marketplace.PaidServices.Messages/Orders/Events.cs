using System;

namespace Marketplace.PaidServices.Messages.Orders
{
    public static class Events
    {
        public static class V1
        {
            public class OrderCreated
            {
                public Guid OrderId { get; set; }
                public Guid ClassifiedAdId { get; set; }
            }

            public class ServiceAddedToOrder
            {
                public Guid OrderId { get; set; }
                public string ServiceType { get; set; }
                public string Description { get; set; }
                public double Price { get; set; }
            }

            public class ServiceRemovedFromOrder
            {
                public Guid OrderId { get; set; }
                public string ServiceType { get; set; }
            }

            public class OrderTotalUpdated
            {
                public Guid OrderId { get; set; }
                public double Totel { get; set; }
            }

            public class OrderFulfilled
            {
                public Guid OrderId { get; set; }
                public Guid ClassifiedAdId { get; set; }
                public string[] Services { get; set; }
            }
        }
    }
}