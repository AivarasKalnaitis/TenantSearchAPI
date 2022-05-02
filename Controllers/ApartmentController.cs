using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Apartments;
using TenantSearchAPI.Data.Dtos.Applications;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/apartments")]
    public class ApartmentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IApartmentsRepository _apartmentRepository;
        private readonly ILandlordsRepository _landlordsRepository;
        private readonly IApplicationsRepository _applicationsRepository;

        public ApartmentController(IMapper mapper, IApartmentsRepository hobbiesRepository, ILandlordsRepository landlordsRepository, IApplicationsRepository applicationsRepository)
        {
            _mapper = mapper;
            _apartmentRepository = hobbiesRepository;
            _landlordsRepository = landlordsRepository;
            _applicationsRepository = applicationsRepository;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<IEnumerable<ApartmentDto>> GetAll(string city, string type, string areaFrom, string areaTo, string roomsFrom, string roomsTo)
        {
            if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(type) && string.IsNullOrEmpty(areaFrom))
            {
                return (await _apartmentRepository.GetAll()).Select(o => _mapper.Map<ApartmentDto>(o));
            }
            else if (!string.IsNullOrEmpty(city))
            {
                return (await _apartmentRepository.GetByCity(city)).Select(o => _mapper.Map<ApartmentDto>(o));
            }
            else if(!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case "flat":
                        return (await _apartmentRepository.GetByType(ApartmentType.FLAT)).Select(o => _mapper.Map<ApartmentDto>(o));
                    case "house":
                        return (await _apartmentRepository.GetByType(ApartmentType.HOUSE)).Select(o => _mapper.Map<ApartmentDto>(o));
                    default:
                        return new List<ApartmentDto>();
                }
            }
            else if(!string.IsNullOrEmpty(areaFrom))
            {
                return (await _apartmentRepository.GetAll()).
                    Where(r => r.Area >= double.Parse(areaFrom) 
                    && r.Area <= double.Parse(areaTo)
                    && r.Rooms >= int.Parse(roomsFrom) && r.Rooms <= int.Parse(roomsTo))
                    .Select(o => _mapper.Map<ApartmentDto>(o));
            }

            return (await _apartmentRepository.GetAll()).Select(o => _mapper.Map<ApartmentDto>(o));
        }

        [HttpGet]
        [Route("landlords/{landlordId}")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<IActionResult> GetByLandlord(Guid landlordId)
        {
            try
            {
                var landlord = await _landlordsRepository.GetById(landlordId);

                if (landlord == null)
                    return BadRequest($"Landlord with id {landlordId} does not exist");

                return Ok((await _apartmentRepository.GetByLandlord(landlordId)).Select(o => _mapper.Map<ApartmentDto>(o)));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("cities")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                return Ok((await _apartmentRepository.GetAll()).Select(a => a.City).Distinct());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{apartmentId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<ApartmentDto>> GetById(Guid apartmentId)
        {
            var appartment = await _apartmentRepository.GetById(apartmentId);

            if (appartment == null)
                return NotFound($"Apartment with id {apartmentId} does not exist");

            return Ok(_mapper.Map<ApartmentDto>(appartment));
        }

        [Authorize(Roles = UserRoles.Landlord)]
        [HttpPost]
        public async Task<ActionResult<ApartmentDto>> Create(CreateApartmentDto apartmentDto)
        {
            var apartment = _mapper.Map<Apartment>(apartmentDto);
            var landlord = await _landlordsRepository.GetById(apartment.LandlordId);

            if(landlord != null)
            {
                var apartments = await _apartmentRepository.GetByLandlord(landlord.Id);
                
                foreach(var singleApartment in apartments)
                {
                    if (singleApartment.City == apartment.City 
                        && string.Compare(singleApartment.Address, apartment.Address, CultureInfo.CurrentCulture, 
                        CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0)
                    {
                        return BadRequest($"{apartment.Type} with address \'{apartment.Address}\' in City \'{apartment.City}\' already declared by landlord \'{landlord.Id}\'.");
                    }
                }

                landlord.Apartments.Add(apartment);
                await _landlordsRepository.Update(landlord);

                return Created($"/api/apartments/{apartment.Id}", _mapper.Map<ApartmentDto>(apartment));
            }

            return BadRequest($"Landlord with id {apartment.LandlordId} does not exist. Create landlord first");
        }

        [HttpPut("{apartmentId}")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult<ApartmentDto>> Update(Guid apartmentId, UpdateApartmentDto apartmentDto)
        {
            var apartment = await _apartmentRepository.GetById(apartmentId);

            if (apartment == null)
                return NotFound($"Apartment with id {apartmentId} doesn not exist");

            _mapper.Map(apartmentDto, apartment);

            await _apartmentRepository.Update(apartment);

            return Ok(_mapper.Map<ApartmentDto>(apartment));
        }

        [HttpDelete("{apartmentId}")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult> Delete(Guid apartmentId)
        {
            var apartment = await _apartmentRepository.GetById(apartmentId);

            if (apartment == null)
                return NotFound($"Apartment with id {apartmentId} doesn not exist");

            await _apartmentRepository.Delete(apartment);

            return NoContent();
        }

        [HttpPost("{apartmentId}/tenants")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult<ApplicationDto>> AddTenant(Guid apartmentId, CreateApplicationDto applicationDto)
        {
            var apartment = await _apartmentRepository.GetById(apartmentId);

            if (apartment == null)
                return NotFound($"Apartment with id {apartmentId} doesn not exist");

            var application = new Application
            {
                Id = Guid.NewGuid(),
                ApartmentId = apartmentId,
                TenantId = Guid.Parse(applicationDto.TenantId),
                SelectedAt = DateTime.Now,
                Status = ApplicationStatus.SELECTED
            };

            await _applicationsRepository.Create(application);

            return Created($"/api/apartments/{apartment.Id}/tenants", _mapper.Map<ApplicationDto>(application));
        }
    }
}
