using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IRecommendationsRepository
    {
        Task<IEnumerable<Apartment>> GetAll();
    }


    public class RecommendationsRepository : IRecommendationsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;

        public RecommendationsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Apartment>> GetAll()
        {
            return await _tenantSearchContext.Apartments.ToListAsync();
        }

    }
}
