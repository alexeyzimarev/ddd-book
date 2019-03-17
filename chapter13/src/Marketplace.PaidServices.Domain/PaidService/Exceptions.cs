using System;

namespace Marketplace.PaidServices.Domain.PaidService
{
    public static class Exceptions
    {
        public class UnknownService : Exception
        {
            public UnknownService(PaidServiceType paidServiceType)
                : base($"Paid service of type {paidServiceType} is unknown") { }
        }
    }
}