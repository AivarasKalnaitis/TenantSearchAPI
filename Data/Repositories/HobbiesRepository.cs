using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IHobbiesRepository
    {
        Task Create(Hobby hobby);
        Task Delete(Hobby hobby);
        Task<IEnumerable<Hobby>> GetAll();
        Task<Hobby> GetById(Guid hobbyId);
        Task<Hobby> GetByName(string hobbyName);
        Task Update(Hobby hobby);
    }


    public class HobbiesRepository : IHobbiesRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public HobbiesRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Hobby>> GetAll()
        {
            return await _tenantSearchContext.Hobbies.ToListAsync();
        }

        public async Task<Hobby> GetById(Guid hobbyId)
        {
            return await _tenantSearchContext.Hobbies.FirstOrDefaultAsync(o => o.Id == hobbyId);
        }

        public async Task<Hobby> GetByName(string hobbyName)
        {
            return await _tenantSearchContext.Hobbies.FirstOrDefaultAsync(h => h.Name.ToLower() == hobbyName.ToLower());
        }

        public async Task Create(Hobby hobby)
        {
            _tenantSearchContext.Hobbies.Add(hobby);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Update(Hobby hobby)
        {
            _tenantSearchContext.Hobbies.Update(hobby);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Hobby hobby)
        {
            _tenantSearchContext.Hobbies.Remove(hobby);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}
