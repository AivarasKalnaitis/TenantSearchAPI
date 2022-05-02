using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;
using System.Linq;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Repositories
{
    public interface ITenantsRepository
    {
        Task Create(Tenant tenant);
        Task Delete(Tenant tenant);
        Task<IEnumerable<Tenant>> GetAll();
        Task<Tenant> GetById(Guid tenantId);
        Task<Tenant> GetByUserId(Guid userId);
        Task<IEnumerable<Tenant>> GetByHobby(Hobby hobby);
        Task<IEnumerable<Tenant>> GetByGender(Gender gender);
        Task Update(Tenant tenant);
        //Task UpdateHobbyTenantTable(List<HobbyTenant> linkingTable);
    }

    public class TenantsRepository : ITenantsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public TenantsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Tenant>> GetAll()
        {
            return await _tenantSearchContext.Tenants.Include(x => x.Hobbies).Include(x => x.Reviews).ToListAsync();
        }

        public async Task<Tenant> GetById(Guid tenantId)
        {
            return await _tenantSearchContext.Tenants.Include(x => x.Hobbies).Include(x => x.Reviews).FirstOrDefaultAsync(o => o.Id == tenantId);
        }

        //public async Task UpdateHobbyTenantTable(List<HobbyTenant> linkingTable)
        //{
        //    var entriesToRemove = await _tenantSearchContext.HobbyTenant.Where(x => x.TenantsId == linkingTable[0].TenantsId).ToListAsync();
        //    _tenantSearchContext.HobbyTenant.RemoveRange(entriesToRemove);
        //    _tenantSearchContext.HobbyTenant.AddRange(linkingTable);

        //    await _tenantSearchContext.SaveChangesAsync();
        //}

        public async Task<Tenant> GetByUserId(Guid userId)
        {
            return await _tenantSearchContext.Tenants.Include(x => x.Hobbies).Include(x => x.Reviews).FirstOrDefaultAsync(o => o.UserId == userId);
        }

        public async Task<IEnumerable<Tenant>> GetByHobby(Hobby hobby)
        {
            return await _tenantSearchContext.Tenants.Include(t => t.Hobbies).Where(t => t.Hobbies.Contains(hobby)).ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetByGender(Gender gender)
        {
            return await _tenantSearchContext.Tenants.Include(t => t.Hobbies).Where(t => t.Gender == gender).ToListAsync();
        }

        public async Task Create(Tenant tenant)
        {
            _tenantSearchContext.Tenants.Add(tenant);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Update(Tenant tenant)
        {
            _tenantSearchContext.Tenants.Update(tenant);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Tenant tenant)
        {
           _tenantSearchContext.Tenants.Remove(tenant);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}