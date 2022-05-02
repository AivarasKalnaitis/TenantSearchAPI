using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.DTOs.Auth;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;

        public UsersController(IMapper mapper, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            return (await _usersRepository.GetAll()).Select(o => _mapper.Map<UserDTO>(o));
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<UserDTO>> GetById(string userId)
        {
            var user = await _usersRepository.GetById(userId);

            if (user == null)
                return NotFound($"User with id {userId} does not exist");

            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> Delete(string userId)
        {
            var user = await _usersRepository.GetById(userId);

            if (user == null)
                return NotFound($"User with id {userId} doesn not exist");

            await _usersRepository.Delete(user);

            return NoContent();
        }
    }
}
