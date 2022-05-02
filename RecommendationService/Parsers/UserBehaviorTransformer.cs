using System;
using System.Collections.Generic;
using System.Linq;
using TenantSearch.Objects;
using TenantSearchAPI;
using UserBehavior.Raters;

namespace TenantSearch.Parsers
{
    public interface IUserBehaviorTransformer
    {
        List<TenantTagsCount> GetTenantTagCounts();
    }
    public class UserBehaviorTransformer : IUserBehaviorTransformer
    {
        private readonly UserBehaviorDatabase _userBehaviorDatabase;

        public UserBehaviorTransformer(UserBehaviorDatabase userBehaviorDatabase)
        {
            _userBehaviorDatabase = userBehaviorDatabase;
        }

        /// Get a list of all landlord and their ratings on every tenant
        public LandlordTenantRatingsTable GetLandlordTenantRatingsTable(LinearRater rater)
        {
            LandlordTenantRatingsTable table = new()
            {
                LandlordIndexesToID = _userBehaviorDatabase.Landlords.OrderBy(x => x.Id).Select(x => x.Id).Distinct().ToList(),
                TenantIndexesToID = _userBehaviorDatabase.Tenants.OrderBy(x => x.Id).Select(x => x.Id).Distinct().ToList()
            };

            foreach (Guid landlordId in table.LandlordIndexesToID)
            {
                // add all tenant indexes for each landlord
                table.Landlords.Add(new LandlordTenantRatings(landlordId, table.TenantIndexesToID.Count));
            }

            //TODO: need to adjust to use landlord apartments history here(when selected, when lived in the apartment)
            //now it takes every landlord action, groups it and creates list with fields LandlordID, TenantId ir Rating

            foreach (var landlordHisotry in _userBehaviorDatabase.LandlordsHistory)
            {
                var test1 = landlordHisotry.Applications.GroupBy(x => new { x.TenantId, x.Status });

                var tenantsRatings = landlordHisotry.Applications.GroupBy(x => new { x.TenantId, x.Status, x.ApartmentId })
                    .Select(g => new { g.Key.TenantId, Rating = rater.GetRating(g.ToList()) });

                foreach(var tenantRating in tenantsRatings)
                {
                    int landlordIndex = table.LandlordIndexesToID.IndexOf(landlordHisotry.LandlordId);
                    int tenantIndex = table.TenantIndexesToID.IndexOf(tenantRating.TenantId);

                    table.Landlords[landlordIndex].TenantRatings[tenantIndex] = tenantRating.Rating;
                }
            }

            return table;
        }

        /// <summary>
        /// Get a table of all tenants as rows and all tags as columns
        /// </summary>
        public List<TenantTagsCount> GetTenantTagCounts()
        {
            List<TenantTagsCount> tenantTags = new();

            foreach (var tenant in _userBehaviorDatabase.Tenants)
            {
                TenantTagsCount tenantTag = new(tenant.Id, UserBehaviorDatabase.TenantTags.Count);

                for (int tag = 0; tag < UserBehaviorDatabase.TenantTags.Count; tag++)
                {
                    // fills TenantTags table in database, when tenants has tag = 1.0, when it dont have it = 0.0

                    if(tenant.Hobbies.Where(x => x.Name.ToUpper() == UserBehaviorDatabase.TenantTags[tag].Name).Any() || Enum.GetName(tenant.Gender).ToUpper() == UserBehaviorDatabase.TenantTags[tag].Name)
                    {
                        tenantTag.TagCounts[tag] = 1.0;
                    }
                    else
                    {
                        tenantTag.TagCounts[tag] = 0.0;
                    }

            }

                tenantTags.Add(tenantTag);
            }

            return tenantTags;
        }

        public static List<T> CreateList<T>()
        {
            return new List<T>();
        }
    }

}