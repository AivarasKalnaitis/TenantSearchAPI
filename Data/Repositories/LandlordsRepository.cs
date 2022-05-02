using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data.Repositories
{
    public interface ILandlordsRepository
    {
        Task Create(Landlord landlord);
        Task Delete(Landlord landlord);
        Task<IEnumerable<Landlord>> GetAll();
        Task<Landlord> GetById(Guid landlordId);
        Task<Landlord> GetByUserId(Guid userId);
        Task Update(Landlord landlord);
    }


    public class LandlordsRepository : ILandlordsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public LandlordsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Landlord>> GetAll()
        {
            return await _tenantSearchContext.Landlords.Include(l => l.Apartments).ToListAsync();
        }

        public async Task<Landlord> GetById(Guid landlordId)
        {
            return await _tenantSearchContext.Landlords.Include(l => l.Apartments).FirstOrDefaultAsync(o => o.Id == landlordId);
        }

        public async Task Create(Landlord landlord)
        {
            _tenantSearchContext.Landlords.Add(landlord);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Update(Landlord landlord)
        {
            _tenantSearchContext.Landlords.Update(landlord);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Landlord landlord)
        {
            _tenantSearchContext.Landlords.Remove(landlord);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task<Landlord> GetByUserId(Guid userId)
        {
            return await _tenantSearchContext.Landlords.Where(l => l.UserId == userId).Include(l => l.Apartments).FirstOrDefaultAsync();   
        }
    }
}
