using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Dtos.Apartments;
using TenantSearchAPI.Data.Dtos.Applications;
using TenantSearchAPI.Data.Dtos.Reviews;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Repositories;
using TenantSearchAPI.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using TenantSearchAPI.Auth.Model;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IApplicationsRepository _applicationRepository;
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IApartmentsRepository _apartmentsRepository;
        private readonly ILandlordsRepository _landlordsRepository;

        public ApplicationController(IMapper mapper, IApplicationsRepository applicationRepository, IApartmentsRepository apartmentsRepository, ITenantsRepository tenantsRepository, ILandlordsRepository landlordsRepository)
        {
            _mapper = mapper;
            _applicationRepository = applicationRepository;
            _apartmentsRepository = apartmentsRepository;
            _tenantsRepository = tenantsRepository;
            _landlordsRepository = landlordsRepository;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IEnumerable<ApplicationDto>> GetAll()
        {
            return (await _applicationRepository.GetAll()).Select(o => _mapper.Map<ApplicationDto>(o));
        }

        [HttpGet]
        [Route("tenants/{tenantId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<List<ApplicationDto>> GetByTenantId(Guid tenantId, string status, string date)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return new List<ApplicationDto>();

            if (string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "0":
                        return (await _applicationRepository.GetByStatusAndTenant(tenantId, ApplicationStatus.PENDING)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "1":
                        return (await _applicationRepository.GetByStatusAndTenant(tenantId, ApplicationStatus.SELECTED)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "2":
                        return (await _applicationRepository.GetByStatusAndTenant(tenantId, ApplicationStatus.CLOSED)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "3":
                        return (await _applicationRepository.GetByStatusAndTenant(tenantId, ApplicationStatus.ACTIVE)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    default:
                        return (await _applicationRepository.GetByTenantId(tenantId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                }
            }
            else if (!string.IsNullOrEmpty(date) && string.IsNullOrEmpty(status))
            {
                if (DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return (await _applicationRepository.GetByDateAndTenant(tenantId, parsedDate)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                }

                return (await _applicationRepository.GetByTenantId(tenantId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
            }

            return (await _applicationRepository.GetByTenantId(tenantId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
        }

        [HttpGet]
        [Route("tenants/{tenantId}/active")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<List<ApplicationDto>> GetActiveByTenantId(Guid tenantId)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return new List<ApplicationDto>();

             return (await _applicationRepository.GetActiveByTenantId(tenantId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();          
        }

        [HttpGet]
        [Route("landlords/{landlordId}/active")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<List<ApplicationDto>> GetActiveByLandlordId(Guid landlordId)
        {
            var landlord = await _landlordsRepository.GetById(landlordId);

            if (landlord == null)
                return new List<ApplicationDto>();

            var apartments = landlord.Apartments;
            List<Guid> apartmentsIds = new List<Guid>();

            foreach(var apartment in apartments)
            {
                apartmentsIds.Add(apartment.Id);
            }

            var landlordApplications = new List<Application>();

            foreach(var id in apartmentsIds)
            {
                var apartmentApplications = (await _applicationRepository.GetActiveByLandlordApartmentId(id));
                landlordApplications.AddRange(apartmentApplications);
            }

            return landlordApplications.Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
        }

        [HttpGet]
        [Route("apartments/{apartmentId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<List<ApplicationDto>> GetByApartmentId(Guid apartmentId, string status, string date)
        {           
            var apartment = await _apartmentsRepository.GetById(apartmentId);

            if (apartment == null)
                return new List<ApplicationDto>();

            if (string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "0":
                        return (await _applicationRepository.GetByStatusAndApartment(apartmentId, ApplicationStatus.PENDING)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "1":
                        return (await _applicationRepository.GetByStatusAndApartment(apartmentId, ApplicationStatus.SELECTED)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "2":
                        return (await _applicationRepository.GetByStatusAndApartment(apartmentId, ApplicationStatus.CLOSED)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    case "3":
                        return (await _applicationRepository.GetByStatusAndApartment(apartmentId, ApplicationStatus.ACTIVE)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                    default:
                        return (await _applicationRepository.GetByApartmentId(apartmentId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                }
            }
            else if (!string.IsNullOrEmpty(date) && string.IsNullOrEmpty(status))
            {
                if(DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return (await _applicationRepository.GetByDateAndApartment(apartmentId, parsedDate)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
                }

                return (await _applicationRepository.GetByApartmentId(apartmentId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
            }

            return (await _applicationRepository.GetByApartmentId(apartmentId)).Select(o => _mapper.Map<ApplicationDto>(o)).ToList();
        }

        [HttpGet("{applicationId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<ApplicationDto>> GetById(Guid applicationId)
        {
            var application = await _applicationRepository.GetById(applicationId);

            if (application == null)
                return NotFound($"Application with id {applicationId} does not exist");

            return Ok(_mapper.Map<ApplicationDto>(application));
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public ActionResult<ApplicationDto> Create(CreateApplicationDto applicationDto)
        {
            var application = _mapper.Map<Application>(applicationDto);
            _applicationRepository.Create(application);

            return Created($"/api/applications", _mapper.Map<ApplicationDto>(application));
        }

        [HttpPut("{applicationId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<ApplicationDto>> Update(Guid applicationId, UpdateApplicationDto updateApplicationDto)
        {
            var application = await _applicationRepository.GetById(applicationId);

            if (application == null)
                return NotFound($"Application with id {applicationId} doesn not exist");

            if (Enum.TryParse(updateApplicationDto.Status, out ApplicationStatus tryParseResult))
            {
                switch (tryParseResult)
                {
                    case ApplicationStatus.CLOSED:
                        application.Status = ApplicationStatus.CLOSED;
                        application.SelectedAt = null;
                        application.ValidUntil = null;
                        break;
                    case ApplicationStatus.PENDING:
                        application.Status = ApplicationStatus.PENDING;
                        application.SelectedAt = null;
                        application.ValidUntil = null;
                        application.AppliedAt = DateTime.Now;
                        break;
                    case ApplicationStatus.SELECTED:
                        application.Status = ApplicationStatus.SELECTED;
                        application.SelectedAt = DateTime.Now;
                        application.AppliedAt = null;
                        break;
                    case ApplicationStatus.ACTIVE:
                        application.Status = ApplicationStatus.ACTIVE;
                        application.SelectedAt = null;
                        application.ValidUntil = DateTime.Now.AddMonths(int.Parse(updateApplicationDto.Months));
                        await DeleteOtherApplicationsAsync(application.TenantId, applicationId);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                return BadRequest("Unkoqn application status");
            }

            await _applicationRepository.Update(application);

            return Ok(_mapper.Map<ApplicationDto>(application));
        }

        [HttpDelete("{applicationId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult> Delete(Guid applicationId)
        {
            var application = await _applicationRepository.GetById(applicationId);

            if (application == null)
                return NotFound($"Application with id {applicationId} doesn not exist");

            await _applicationRepository.Delete(application);

            return NoContent();
        }

        private async Task DeleteOtherApplicationsAsync(Guid tenantId, Guid applicationId)
        {
            var applications = (await _applicationRepository.GetByTenantId(tenantId)).Where(a => a.Id != applicationId);

            foreach(var application in applications)
            {
                await _applicationRepository.Delete(application);
            }
        }
    }
}
