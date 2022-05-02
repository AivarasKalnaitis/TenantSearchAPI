using System;
using System.Collections.Generic;
using System.Linq;
using TenantSearch.Objects;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;
using TenantSearchAPI.RecommendationService.Objects;

namespace UserBehavior.Raters
{
    /// <summary>
    /// Uses linear function to rate tenants ?
    /// </summary>
    public class LinearRater
    {
        private readonly double viewWeight;
        private readonly double selectWeight;

        private readonly double minWeight;
        private readonly double maxWeight;


        public LinearRater(double view, double select) : this(view, select, 5.0)
        {
        }

        public LinearRater(double selected, double pending, double max)
        {
            viewWeight = selected;
            selectWeight = pending;

            minWeight = 0;
            maxWeight = max;
        }

        // will need to change or delete this method later, because we will not use actions ?
        public double GetRating(List<Application> applications)
        {
            int selectedNumber = applications.Count(x => x.Status == ApplicationStatus.CLOSED || x.Status == ApplicationStatus.SELECTED);
            int pendingNumber = applications.Count(x => x.Status == ApplicationStatus.PENDING);

            double rating = selectedNumber * 2.5 + pendingNumber * 0.5;

            return Math.Min(maxWeight, Math.Max(minWeight, rating));
        }
    }
}
