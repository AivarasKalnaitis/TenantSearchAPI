using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IApartmentsRepository
    {
        Task Delete(Apartment apartment);
        Task<IEnumerable<Apartment>> GetAll();
        Task<IEnumerable<Apartment>> GetByLandlord(Guid landlordId);
        Task<IEnumerable<Apartment>> GetByCity(string city);
        Task<IEnumerable<Apartment>> GetByType(ApartmentType type);
        Task<Apartment> GetById(Guid apartmentId);
        Task Update(Apartment apartment);
    }


    public class ApartmentsRepository : IApartmentsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public ApartmentsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Apartment>> GetAll()
        {
            return await _tenantSearchContext.Apartments.ToListAsync();
        }

        public async Task<IEnumerable<Apartment>> GetByType(ApartmentType type)
        {
            return await _tenantSearchContext.Apartments.Where(t => t.Type == type).ToListAsync();

        }

        public async Task<IEnumerable<Apartment>> GetByLandlord(Guid landlordId)
        {
            return await _tenantSearchContext.Apartments.Where(a => a.LandlordId == landlordId).ToListAsync();
        }
        public async Task<IEnumerable<Apartment>> GetByCity(string city)
        {
            return await _tenantSearchContext.Apartments.Where(a => a.City.ToLower() == city.ToLower()).ToListAsync();
        }

        public async Task<Apartment> GetById(Guid apartmentId)
        {
            return await _tenantSearchContext.Apartments.FirstOrDefaultAsync(o => o.Id == apartmentId);
        }

        public async Task Update(Apartment apartment)
        {
            _tenantSearchContext.Apartments.Update(apartment);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Apartment apartment)
        {
            await _tenantSearchContext.TenantApplications.Where(a => a.ApartmentId == apartment.Id).ForEachAsync(x => _tenantSearchContext.TenantApplications.Remove(x));

            var userId = _tenantSearchContext.Landlords.FirstOrDefault(l => l.Id == apartment.LandlordId).UserId;

            _tenantSearchContext.Users.Where(u => Guid.Parse(u.Id) == userId).Select(u => _tenantSearchContext.Users.Remove(u));

            _tenantSearchContext.Apartments.Remove(apartment);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}
