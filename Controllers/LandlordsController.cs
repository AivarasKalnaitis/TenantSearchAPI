using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Applications;
using TenantSearchAPI.Data.Dtos.Landlords;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/landlords")]
    public class LandlordsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILandlordsRepository _landlordsRepository;
        private readonly IApartmentsRepository _apartmentsRepository;
        private readonly IApplicationsRepository _applicationsRepository;

        public LandlordsController(IMapper mapper, ILandlordsRepository landlordsRepository, IApartmentsRepository apartmentsRepository, IApplicationsRepository applicationsRepository)
        {
            _mapper = mapper;
            _landlordsRepository = landlordsRepository;
            _apartmentsRepository = apartmentsRepository;
            _applicationsRepository = applicationsRepository;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IEnumerable<LandlordDto>> GetAll()
        {
            return (await _landlordsRepository.GetAll()).Select(o => _mapper.Map<LandlordDto>(o));
        }

        [HttpGet("{landlordId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<LandlordDto>> GetById(Guid landlordId)
        {
            var landlord = await _landlordsRepository.GetById(landlordId);

            if (landlord == null)
                return NotFound($"Landlord with id {landlordId} does not exist");

            return Ok(_mapper.Map<LandlordDto>(landlord));
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<LandlordDto>> GetByUserId(Guid userId)
        {
            var landlord = await _landlordsRepository.GetByUserId(userId);

            if (landlord == null)
                return NotFound($"Landlord with userId {User} does not exist");

            return Ok(_mapper.Map<LandlordDto>(landlord));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LandlordDto>> Create(CreateLandlordDto landlordDto)
        {
            var landlord = _mapper.Map<Landlord>(landlordDto);
            landlord.Name = char.ToUpper(landlord.Name[0]) + landlord.Name[1..];
            landlord.Surname = char.ToUpper(landlord.Surname[0]) + landlord.Surname[1..];

            var landlords = await _landlordsRepository.GetAll();

            foreach (var singleLandlord in landlords)
            {
                if (singleLandlord.Name == landlord.Name && singleLandlord.Surname == landlord.Surname)
                {
                    return BadRequest($"Landlord \'{landlord.Name} {landlord.Surname}\' already exists.");
                }
            }

            var apartments = await _apartmentsRepository.GetAll();

            foreach (var landlordApartment in landlord.Apartments)
            {
                foreach (var singleApartment in apartments)
                {
                    if (singleApartment.City == singleApartment.City
                        && string.Compare(singleApartment.Address, landlordApartment.Address, CultureInfo.CurrentCulture,
                        CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0)
                    {
                        return BadRequest($"{singleApartment.Type} with address \'{singleApartment.Address}\' in City \'{singleApartment.City}\' already declared by landlord \'{singleApartment.LandlordId}\'.");
                    }
                }
            }

            await _landlordsRepository.Create(landlord);

            return Created($"/api/landlords/{landlord.Id}", _mapper.Map<LandlordDto>(landlord));
        }

        [HttpPut("{landlordId}")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult<LandlordDto>> Update(Guid landlordId, UpdateLandlordDto landlordDto)
        {
            var landlord = await _landlordsRepository.GetById(landlordId);

            if (landlord == null)
                return NotFound($"Landlord with id {landlordId} doesn not exist");

            _mapper.Map(landlordDto, landlord);

            await _landlordsRepository.Update(landlord);

            return Ok(_mapper.Map<LandlordDto>(landlord));
        }

        [HttpDelete("{landlordId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> Delete(Guid landlordId)
        {
            var landlord = await _landlordsRepository.GetById(landlordId);

            if (landlord == null)
                return NotFound($"Landlord with id {landlordId} doesn not exist");

            await _landlordsRepository.Delete(landlord);

            return NoContent();
        }

        [HttpGet("{landlordId}/history")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<ActionResult<ApplicationDto>> GetHistory(Guid landlordId)
        {
            var landlord = await _landlordsRepository.GetById(landlordId);

            if (landlord == null)
                return BadRequest($"Landlord with id {landlordId} does not exist.");

            var apartmentsIds = (await _apartmentsRepository.GetByLandlord(landlordId)).Select(a => a.Id);
            var applications = await _applicationsRepository.GetAll();
            var landlordApplications = new List<Application>();

            foreach(var appartmentId in apartmentsIds)
            {
                var temp = applications.Where(x => x.ApartmentId == appartmentId).FirstOrDefault();

                if (temp != null)
                {
                    landlordApplications.Add(temp);
                }
            }


            return Ok(landlordApplications.Select(o => _mapper.Map<ApplicationDto>(o)));
        }
    }
}
