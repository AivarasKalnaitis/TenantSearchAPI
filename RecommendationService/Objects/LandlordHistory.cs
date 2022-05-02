using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.RecommendationService.Objects
{
    public class LandlordHistory
    {
        public Guid LandlordId;
        public List<Application> Applications;

        public LandlordHistory(Guid id)
        {
            LandlordId = id;
        }

        public LandlordHistory(Guid id, List<Application> applications)
        {
            LandlordId = id;
            Applications = applications;
        }
    }
}
