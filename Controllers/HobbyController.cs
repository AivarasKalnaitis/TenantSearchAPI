using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Hobbies;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/hobbies")]
    public class HobbyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHobbiesRepository _hobbiesRepository;

        public HobbyController(IMapper mapper, IHobbiesRepository hobbiesRepository)
        {
            _mapper = mapper;
            _hobbiesRepository = hobbiesRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<HobbyDto>> GetAll()
        {
            return (await _hobbiesRepository.GetAll()).Select(o => _mapper.Map<HobbyDto>(o));
        }

        [HttpGet("{hobbyId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<HobbyDto>> GetById(Guid hobbyId)
        {
            var hobby = await _hobbiesRepository.GetById(hobbyId);

            if (hobby == null)
                return NotFound($"Hobby with id \'{hobbyId}\' does not exist.");

            return Ok(_mapper.Map<HobbyDto>(hobby));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<ActionResult<HobbyDto>> Create(CreateHobbyDto hobbyDto)
        {
            var hobby = _mapper.Map<Hobby>(hobbyDto);
            hobby.Name = char.ToUpper(hobby.Name[0]) + hobby.Name[1..];

            var doesHobbyExist = (await _hobbiesRepository.GetAll())
                .Select(h => h.Name).ToList()
                .Contains(hobbyDto.Name);

            if (!doesHobbyExist)
            {
                await _hobbiesRepository.Create(hobby);

                return Created($"/api/hobbies/{hobby.Id}", _mapper.Map<HobbyDto>(hobby));
            }

            return BadRequest($"Hobby \'{hobby.Name}\' already exists.");
        }

        [HttpPut("{hobbyId}")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<ActionResult<HobbyDto>> Update(Guid hobbyId, UpdateHobbyDto hobbyDto)
        {
            var hobby = await _hobbiesRepository.GetById(hobbyId);

            if (hobby == null)
                return NotFound($"Hobby \'{hobbyId}\' does not exist.");

            _mapper.Map(hobbyDto, hobby);

            await _hobbiesRepository.Update(hobby);

            return Ok(_mapper.Map<HobbyDto>(hobby));
        }

        [HttpDelete("{hobbyId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> Delete(Guid hobbyId)
        {
            var hobby = await _hobbiesRepository.GetById(hobbyId);

            if (hobby == null)
                return NotFound($"Hobby \'{hobbyId}\' does not exist.");

            await _hobbiesRepository.Delete(hobby);

            return NoContent();
        }
    }
}
