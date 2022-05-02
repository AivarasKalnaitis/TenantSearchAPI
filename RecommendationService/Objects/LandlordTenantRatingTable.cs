using System;
using System.Collections.Generic;
using System.Drawing;

namespace TenantSearch.Objects
{
    public class LandlordTenantRatingsTable
    {
        public List<LandlordTenantRatings> Landlords { get; set; }

        public List<Guid> LandlordIndexesToID { get; set; }

        public List<Guid> TenantIndexesToID { get; set; }

        public LandlordTenantRatingsTable()
        {
            Landlords = new List<LandlordTenantRatings>();
        }

        /// <summary>
        /// Adds tenant tags rows in main data table (at the start it has 9 landlord rows, but here it adds 10 more tenant tags rows filled with 0-1)
        /// </summary>
        public void AppendTenantFeatures(double[][] tenantFeatures)
        {
            for (int f = 0; f < tenantFeatures[0].Length; f++)
            {
                LandlordTenantRatings newFeature = new LandlordTenantRatings(Guid.Empty, TenantIndexesToID.Count);

                for (int a = 0; a < TenantIndexesToID.Count; a++)
                {
                    newFeature.TenantRatings[a] = tenantFeatures[a][f];
                }

                Landlords.Add(newFeature);
            }
        }

        internal void AppendTenantFeatures(List<TenantTagsCount> tenantTags)
        {
            double[][] features = new double[tenantTags.Count][];

            for (int t = 0; t < tenantTags.Count; t++)
            {
                features[t] = new double[tenantTags[t].TagCounts.Length];

                for (int f = 0; f < tenantTags[t].TagCounts.Length; f++)
                {
                    features[t][f] = tenantTags[t].TagCounts[f];
                }
            }

            AppendTenantFeatures(features);
        }
    }

}
