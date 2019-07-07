using System;
using System.Collections.Generic;

namespace Marketplace.PaidServices.Queries.ClassifiedAds
{
    public static class ReadModels
    {
        public class AdActiveServices
        {
            public Guid ClassifiedAdId { get; set; }
            public List<Service> ActiveServices { get; set; }

            public class Service
            {
                public string ServiceType { get; set; }
            }
            
            public static string GetDatabaseId(Guid id)
                => $"ClassifiedAdActiveServices/{id}";
        }

        public class ClassifiedAdOrders
        {
            public string Id { get; set; }
            public string CustomerId { get; set; }
            public List<Order> Orders { get; set; }

            public class Order
            {
                public string OrderId { get; set; }
                public double Total { get; set; }
                public DateTimeOffset FulfilledAt { get; set; }
            }
            
            public static string GetDatabaseId(Guid id)
                => $"ClassifiedAdOrders/{id}";
        }
    }
}
