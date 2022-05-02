using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Repositories
{
    public interface IApplicationsRepository
    {
        Task<IEnumerable<Application>> GetAll();

        Task<IEnumerable<Application>> GetByApartmentId(Guid apartmentId);
        Task<IEnumerable<Application>> GetByTenantId(Guid tenantId);
        Task Create(Application application);
        Task<Application> GetById(Guid applicationId);
        Task Update(Application application);
        Task Delete(Application application);
        Task<IEnumerable<Application>> GetByStatusAndApartment(Guid apartmentId, ApplicationStatus status);
        Task<IEnumerable<Application>> GetActiveByTenantId(Guid tenantId);
        Task<IEnumerable<Application>> GetActiveByLandlordApartmentId(Guid apartmentId);
        Task<IEnumerable<Application>> GetByStatusAndTenant(Guid tenantId, ApplicationStatus status);
        Task<IEnumerable<Application>> GetByDateAndApartment(Guid apartmentId, DateTime date);
        Task<IEnumerable<Application>> GetByDateAndTenant(Guid tenantId, DateTime date);
    }


    public class ApplicationsRepository : IApplicationsRepository
    {
        private readonly TenantSearchContext _tenantSearchContext;
        public ApplicationsRepository(TenantSearchContext tenantSearchContext)
        {
            _tenantSearchContext = tenantSearchContext;
        }

        public async Task<IEnumerable<Application>> GetByStatusAndApartment(Guid apartmentId, ApplicationStatus status)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => a.Status == status && a.ApartmentId == apartmentId).ToListAsync();
        }
        public async Task<IEnumerable<Application>> GetActiveByTenantId(Guid tenantId)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => (a.Status == ApplicationStatus.PENDING || a.Status == ApplicationStatus.SELECTED || a.Status == ApplicationStatus.ACTIVE) && a.TenantId == tenantId).ToListAsync();
        }
        public async Task<IEnumerable<Application>> GetActiveByLandlordApartmentId(Guid apartmentId)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => (a.Status == ApplicationStatus.PENDING || a.Status == ApplicationStatus.SELECTED || a.Status == ApplicationStatus.ACTIVE) && a.ApartmentId == apartmentId).ToListAsync();
        }
        public async Task<IEnumerable<Application>> GetByStatusAndTenant(Guid tenantId, ApplicationStatus status)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => a.Status == status && a.TenantId == tenantId).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByDateAndApartment(Guid apartmentId, DateTime date)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => (a.AppliedAt >= date || a.SelectedAt >= date) && a.ApartmentId == apartmentId).ToListAsync();
        }
        public async Task<IEnumerable<Application>> GetByDateAndTenant(Guid tenantId, DateTime date)
        {
            return await _tenantSearchContext.TenantApplications.Where(a => (a.AppliedAt >= date || a.SelectedAt >= date) && a.TenantId == tenantId).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetAll()
        {
            return await _tenantSearchContext.TenantApplications.ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByApartmentId(Guid apartmentId)
        {
            return await _tenantSearchContext.TenantApplications.Where(x => x.ApartmentId == apartmentId).ToListAsync();
        }
        public async Task<IEnumerable<Application>> GetByTenantId(Guid tenantId)
        {
            return await _tenantSearchContext.TenantApplications.Where(x => x.TenantId == tenantId).ToListAsync();
        }

        public async Task<Application> GetById(Guid applicationId)
        {
            return await _tenantSearchContext.TenantApplications.FirstOrDefaultAsync(o => o.Id == applicationId);
        }

        public async Task Create(Application application)
        {
            _tenantSearchContext.TenantApplications.Add(application);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Update(Application application)
        {
            _tenantSearchContext.TenantApplications.Update(application);

            await _tenantSearchContext.SaveChangesAsync();
        }

        public async Task Delete(Application application)
        {
            _tenantSearchContext.TenantApplications.Remove(application);

            await _tenantSearchContext.SaveChangesAsync();
        }
    }
}
