using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantSearchAPI.RecommendationService.Objects
{
    public class LandlordTenantRatingByApplications
    {
        public Guid LandlordID { get; set; }
        public Guid TenantId { get; set; }
        public double Rating { get; set; }
    }
}
