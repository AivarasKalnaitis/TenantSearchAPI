using System;
using System.Collections.Generic;

namespace TenantSearch.Objects
{
    public class LandlordTenantRatings
    {
        public Guid LandlordId { get; set; }

        public double[] TenantRatings { get; set; }

        public double Score { get; set; }

        public LandlordTenantRatings(Guid landlordId, int tenantsCount)
        {
            LandlordId = landlordId;
            TenantRatings = new double[tenantsCount];
        }

        public void AppendRatings(double[] ratings)
        {
            List<double> allRatings = new List<double>();

            allRatings.AddRange(TenantRatings);
            allRatings.AddRange(ratings);

            TenantRatings = allRatings.ToArray();
        }
    }
}
