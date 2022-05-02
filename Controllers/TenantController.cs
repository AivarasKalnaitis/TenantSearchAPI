using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Landlords;
using TenantSearchAPI.Data.Dtos.Tenants;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IHobbiesRepository _hobbiesRepository;
        private readonly IUsersRepository _usersRepository;

        public TenantController(IMapper mapper, ITenantsRepository tenantsRepository, IHobbiesRepository hobbiesRepository, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _tenantsRepository = tenantsRepository;
            _hobbiesRepository = hobbiesRepository;
            _usersRepository = usersRepository;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<IEnumerable<TenantDto>> GetAll(string hobby, string gender)
        {
            if (string.IsNullOrEmpty(hobby) && string.IsNullOrEmpty(gender))
            {
                return (await _tenantsRepository.GetAll()).Select(o => _mapper.Map<TenantDto>(o));
            }
            else if(!string.IsNullOrEmpty(hobby))
            {
                var hobbyObject = await _hobbiesRepository.GetByName(hobby);

                if(hobbyObject == null)
                {
                    return new List<TenantDto>();
                }

                return (await _tenantsRepository.GetByHobby(hobbyObject)).Select(o => _mapper.Map<TenantDto>(o));
            }
            else
            {
                switch(gender)
                {
                    case "male":
                        return (await _tenantsRepository.GetByGender(Gender.MALE)).Select(o => _mapper.Map<TenantDto>(o));
                    case "female":
                        return (await _tenantsRepository.GetByGender(Gender.FEMALE)).Select(o => _mapper.Map<TenantDto>(o));
                    default:
                        return new List<TenantDto>();
                }
            }
        }

        [HttpGet("{tenantId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<TenantDto>> GetById(Guid tenantId)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return NotFound($"Tenant with id {tenantId} does not exist.");

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpGet("users/{userId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<TenantDto>> GetByUserId(Guid userId)
        {
            var tenant = await _tenantsRepository.GetByUserId(userId);

            if (tenant == null)
                return NotFound($"Tenant with user id {userId} does not exist.");

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TenantDto>> Create(CreateTenantDto tenantDTO)
        {
            var tenant = _mapper.Map<Tenant>(tenantDTO);
            tenant.Name = char.ToUpper(tenant.Name[0]) + tenant.Name[1..];
            tenant.Surname = char.ToUpper(tenant.Surname[0]) + tenant.Surname[1..];

            var tenants = await _tenantsRepository.GetAll();

            foreach (var singleTenant in tenants)
            {
                if (singleTenant.Name == tenant.Name && singleTenant.Surname == tenant.Surname)
                {
                    return BadRequest($"Tenant \'{tenant.Name} {tenant.Surname}\' already exists.");
                }
            }

            var hobbies = await _hobbiesRepository.GetAll();

            foreach (var tenantHobby in tenant.Hobbies)
            {
                foreach(var singleHobby in hobbies)
                {
                    if(tenantHobby.Name.ToUpper() == singleHobby.Name.ToUpper())
                    {
                        return BadRequest($"Hobby \'{tenantHobby.Name}\' already exists. Please remove it from your list.");
                    }
                }
            }

            await _tenantsRepository.Create(tenant);

            return Created($"/api/tenants/{tenant.Id}", _mapper.Map<TenantDto>(tenant));
        }

        [HttpPut("{tenantId}")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<ActionResult<TenantDto>> Update(Guid tenantId, UpdateTenantDto tenantDTO)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return NotFound($"Tenant with id {tenantId} doesn not exist.");

            var oldHobbies = tenant.Hobbies;
            var newHobbies = tenantDTO.Hobbies;

            foreach(var hobby in oldHobbies.ToList())
            {
                if(!newHobbies.Contains(hobby.Name))
                {
                    await CheckIfDeleteNeededAsync(hobby, tenantId);
                }
            }

            var hobbiesToAdd = await GetHobbiesAsync(tenantDTO.Hobbies);

            tenant.Hobbies = hobbiesToAdd;
            tenant.Gender = tenantDTO.Gender == 0 ? Gender.MALE : Gender.FEMALE;
            tenant.Name = tenantDTO.Name;
            tenant.Surname = tenantDTO.Surname;

            await _tenantsRepository.Update(tenant);

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpDelete("{tenantId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> Delete(Guid tenantId)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return NotFound($"Tenant with id {tenantId} doesn not exist.");

            var user = await _usersRepository.GetById(tenant.UserId.ToString());

            var hobbiesToCheck = tenant.Hobbies;
            var tenants = (await _tenantsRepository.GetAll()).Where(t => t.Id != tenantId);

            foreach (var hobby in hobbiesToCheck.ToList())
            {
                bool found = false;
                foreach (var ten in tenants)
                {
                    if (ten.Hobbies.Contains(hobby))
                        found = true;
                }

                if (!found)
                    await _hobbiesRepository.Delete(hobby);
            }

            await _tenantsRepository.Delete(tenant);
            await _usersRepository.Delete(user);

            return NoContent();
        }

        private async Task<List<Hobby>> GetHobbiesAsync(List<string> hobbiesNames)
        {
            var hobbies = new List<Hobby>();

            var existingHobbies = await _hobbiesRepository.GetAll();


            foreach (string hobbyName in hobbiesNames)
            {
                if (existingHobbies.Any(h => h.Name == hobbyName))
                {
                    var id = existingHobbies.Where(h => h.Name == hobbyName).First().Id;
                    hobbies.Add(await _hobbiesRepository.GetById(id));
                }

                else
                {
                    hobbies.Add(new Hobby
                    {
                        Name = hobbyName,
                    });
                }
            }

            return hobbies;
        }

        private async Task CheckIfDeleteNeededAsync(Hobby hobby, Guid tenantId)
        {
            var tenants = (await _tenantsRepository.GetAll()).Where(t => t.Id != tenantId);
            bool hobbyFound = false;

            foreach(var tenant in tenants)
            {
                if (tenant.Hobbies.Contains(hobby))
                {
                    hobbyFound = true;
                    break;
                }
            }

            if(!hobbyFound)
            {
                await _hobbiesRepository.Delete(hobby);
            }
        }
    }
}
