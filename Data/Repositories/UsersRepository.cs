using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.DTOs.Auth;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(string userId);
        Task Update(User user);
        Task Delete(User user);
    }


    public class UsersRepository : IUsersRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public UsersRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _tenantSearchContext.Users.ToListAsync();
        }

        public async Task<User> GetById(string userId)
        {
            return await _tenantSearchContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        }

        public async Task Update(User user)
        {
            _tenantSearchContext.Users.Update(user);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            _tenantSearchContext.Users.Remove(user);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}
