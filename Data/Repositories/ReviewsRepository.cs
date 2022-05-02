using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IReviewsRepository
    {
        Task Delete(Review review);
        Task<IEnumerable<Review>> GetAll(Guid tenantId);
        Task<IEnumerable<Review>> GetAllFromAllTenants();
        Task<Review> GetById(Guid reviewId);
        Task Update(Review review);
    }


    public class ReviewsRepository : IReviewsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public ReviewsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }
        public async Task<IEnumerable<Review>> GetAllFromAllTenants()
        {
            return await _tenantSearchContext.Reviews.ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAll(Guid tenandId)
        {
            return await _tenantSearchContext.Reviews.Where(x => x.TenantId == tenandId).ToListAsync();
        }

        public async Task<Review> GetById(Guid reviewId)
        {
            return await _tenantSearchContext.Reviews.FirstOrDefaultAsync(o => o.Id == reviewId);
        }

        public async Task Update(Review review)
        {
            _tenantSearchContext.Reviews.Update(review);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Review review)
        {
            _tenantSearchContext.Reviews.Remove(review);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}
