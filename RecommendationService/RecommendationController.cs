using TenantSearch.Comparers;
using TenantSearch.Recommenders;
using UserBehavior.Raters;
using TenantSearch.Objects;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using TenantSearch.Parsers;
using System.Linq;

namespace TenantSearch
{
    [ApiController]
    [Route("api/recommender")]
    public class RecommendationController : ControllerBase
    {
        private readonly TenantRecommender recommender;
        private readonly UserBehaviorDatabase _userBehaviorDatabase;
        private readonly IUserBehaviorTransformer _userBehaviorTransformer;

        public RecommendationController(UserBehaviorDatabase userBehaviorDatabase, IUserBehaviorTransformer userBehaviorTransformer)
        {
            _userBehaviorDatabase = userBehaviorDatabase;
            _userBehaviorTransformer = userBehaviorTransformer;
            var rate = new LinearRater(0.5, 2);
            var compare = new TenantComparer();

            recommender = new TenantRecommender(_userBehaviorTransformer, compare, rate);
        }

        [HttpGet]
        [Route("train")]
        public async Task Train()
        {
            await _userBehaviorDatabase.RetrieveAllData();
            recommender.Train(_userBehaviorDatabase);
        }

        [HttpGet]
        [Route("recommend")]
        public async Task<ActionResult> Recommend(Guid landlordId)
        {
            await Train();
            // handle out of range exceptions

            List<Suggestion> suggestions = recommender.GetSuggestions(landlordId);
            suggestions = suggestions.OrderByDescending(x => x.Rating).ToList();

            if (suggestions.Count > 0)
            {
                List<Suggestion> result = new();

                foreach (var suggestion in suggestions)
                {
                    result.Add(new Suggestion(suggestion.LandlordId, suggestion.TenantId, suggestion.Rating));
                    //result += $"TenantId: {suggestion.TenantId} (score: {suggestion.Rating})\n";
                }

                return Ok(result);
            }
            else
            {
                return BadRequest($"Landlord ID: {landlordId} does not have any views or selections yet. Will recommend random tenants later . . .");
            }
        }
        //else
        //{
        //    return BadRequest("Invalid User ID or Recommendation Count!");
        //}
    }
}
