using System;

namespace TenantSearch.Objects
{
    public class Suggestion
    {
        public Guid LandlordId { get; set; }

        public Guid TenantId { get; set; }

        public double Rating { get; set; }

        public Suggestion(Guid landlordId, Guid tenantId, double assurance)
        {
            // check null - throw exception
            //LandlordId = landlordId ?? throw new ArgumentNullException();
            LandlordId = landlordId;
            TenantId = tenantId;
            Rating = assurance;
        }
    }
}