using System;

namespace TenantSearch.Objects
{
    public class TenantRating
    {
        public Guid TenantId { get; set; }

        public double Rating { get; set; }

        public TenantRating(Guid tenantId, double rating)
        {
            TenantId = tenantId;
            Rating = rating;
        }
    }
}