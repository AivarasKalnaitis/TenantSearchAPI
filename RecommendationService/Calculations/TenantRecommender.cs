using System;
using System.Collections.Generic;
using System.Linq;
using TenantSearch.Comparers;
using TenantSearch.Objects;
using TenantSearch.Parsers;
using UserBehavior.Raters;

namespace TenantSearch.Recommenders
{
    public class TenantRecommender
    {
        private readonly IUserBehaviorTransformer _userBehaviorTransformer;
        private readonly TenantComparer comparer;
        private readonly LinearRater rater;
        private LandlordTenantRatingsTable ratings = new LandlordTenantRatingsTable();
        private double[][] transposedRatings;

        public TenantRecommender(IUserBehaviorTransformer userBehaviorTransformer, TenantComparer itemComparer, LinearRater implicitRater)
        {
            _userBehaviorTransformer = userBehaviorTransformer;
            comparer = itemComparer;
            rater = implicitRater;
        }

        private void FillTransposedRatings()
        {
            int features = ratings.Landlords.Count;
            transposedRatings = new double[ratings.TenantIndexesToID.Count][];

            // Precompute a transposed ratings matrix where each row becomes an tenant and each column a landlord or tag
            for (int a = 0; a < ratings.TenantIndexesToID.Count; a++)
            {
                transposedRatings[a] = new double[features];

                for (int f = 0; f < features; f++)
                {
                    transposedRatings[a][f] = ratings.Landlords[f].TenantRatings[a];
                }
            }
        }

        private List<int> GetHighestRatedTenantsForLandlord(int landlordIndex)
        {
            var items = new List<Tuple<int, double>>();

            for (int tenantIndex = 0; tenantIndex < ratings.TenantIndexesToID.Count; tenantIndex++)
            {
                // Create a list of every tenant this landlord has applied or selected
                if (ratings.Landlords[landlordIndex].TenantRatings[tenantIndex] != 0)
                {
                    items.Add(new Tuple<int, double>(tenantIndex, ratings.Landlords[landlordIndex].TenantRatings[tenantIndex]));
                }
            }

            // Sort the tenants by rating
            items.Sort((c, n) => n.Item2.CompareTo(c.Item2));

            return items.Select(x => x.Item1).ToList();
        }

        public void Train(UserBehaviorDatabase userBehaviorDatabase)
        {
            var ubt = new UserBehaviorTransformer(userBehaviorDatabase);
            ratings = ubt.GetLandlordTenantRatingsTable(rater);

            List<TenantTagsCount> tenantTags = ubt.GetTenantTagCounts();
            ratings.AppendTenantFeatures(tenantTags);

            FillTransposedRatings();
        }

        public List<Suggestion> GetSuggestions(Guid landlordId, int numSuggestions = 5)
        {
            var landlordIndex = ratings.LandlordIndexesToID.ToList().IndexOf(landlordId);
            var tenants = GetHighestRatedTenantsForLandlord(landlordIndex).Take(5).ToList();
            var suggestions = new List<Suggestion>();

            foreach (int tenantIndex in tenants)
            {
                var tenantId = ratings.TenantIndexesToID[tenantIndex];
                var neighboringTenants = GetNearestNeighbors(tenantId);

                foreach (TenantRating neighbor in neighboringTenants)
                {
                    int neighborTenantIndex = ratings.TenantIndexesToID.IndexOf(neighbor.TenantId);

                    //var nonZeroRatings = transposedRatings[neighborTenantIndex].Where(x => x != 0);
                    //double averageTenantRating = nonZeroRatings.Any() ? nonZeroRatings.Average() : 0;

                    double averageTenantRating = 0.0;
                    int count = 0;
                    for (int tagIndex = 0; tagIndex < ratings.Landlords.Count; tagIndex++)
                    {
                        if (transposedRatings[neighborTenantIndex][tagIndex] != 0)
                        {
                            averageTenantRating += transposedRatings[neighborTenantIndex][tagIndex];
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        averageTenantRating /= count;
                    }

                    suggestions.Add(new Suggestion(landlordId, neighbor.TenantId, averageTenantRating));
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            var test = suggestions.GroupBy(x => new { x.TenantId, x.Rating }).Select(g => new Suggestion(landlordId, g.Key.TenantId, g.Key.Rating)).ToList();
            test.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return test.Take(numSuggestions).ToList();
        }

        private List<TenantRating> GetNearestNeighbors(Guid tenantId, int numTenants = 5)
        {
            List<TenantRating> neighbors = new();
            int mainTenantIndex = ratings.TenantIndexesToID.IndexOf(tenantId);

            for (int tenantIndex = 0; tenantIndex < ratings.TenantIndexesToID.Count; tenantIndex++)
            {
                var searchTenantId = ratings.TenantIndexesToID[tenantIndex];

                double score = TenantComparer.CompareVectors(transposedRatings[mainTenantIndex], transposedRatings[tenantIndex]);

                neighbors.Add(new TenantRating(searchTenantId, score));
            }

            neighbors.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return neighbors.Take(numTenants).ToList();
        }
    }
}
