using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TenantSearchAPI.Auth;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.DTOs.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using RelicsAPI.Auth;
using TenantSearchAPI.Data.Repositories;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace RelicsAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenManager _tokenManager;
        private readonly ITenantsRepository _tenantsRepository;
        private readonly ILandlordsRepository _landlordsRepository;
        private readonly IHobbiesRepository _hobbiesRepository;

        public AuthController(UserManager<User> userManager, IMapper mapper, ITokenManager tokenManager, ITenantsRepository tenantsRepository, ILandlordsRepository landlordsRepository, IHobbiesRepository hobbiesRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenManager = tokenManager;
            _landlordsRepository = landlordsRepository;
            _tenantsRepository = tenantsRepository;
            _hobbiesRepository = hobbiesRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterUserDto registerUserDTO)
        {
            var user = await _userManager.FindByEmailAsync(registerUserDTO.Email);

            if (user != null)
                return BadRequest("Such email already in use !");

            var newUser = new User
            {
                Email = registerUserDTO.Email,
                UserName = registerUserDTO.Name
            };

            var createdUserResult = await _userManager.CreateAsync(newUser, registerUserDTO.Password);

            if (!createdUserResult.Succeeded)
                return BadRequest("Name already in use !");

            switch (registerUserDTO.RoleOfUser)
            {
                case "Tenant":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Tenant);
                    await _tenantsRepository.Create(new Tenant
                    {
                        Name = registerUserDTO.Name,
                        Surname = registerUserDTO.Surname,
                        Gender = registerUserDTO.Gender == "0" ? Gender.MALE : Gender.FEMALE,
                        Hobbies = await GetHobbiesAsync(registerUserDTO.Hobbies),
                        UserId = Guid.Parse(newUser.Id),
                    });
                    break;
                case "Landlord":
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Landlord);
                    await _landlordsRepository.Create(new Landlord
                    {
                        Name = registerUserDTO.Name,
                        Surname = registerUserDTO.Surname,
                        Gender = registerUserDTO.Gender == "0" ? Gender.MALE : Gender.FEMALE,
                        UserId = Guid.Parse(newUser.Id),
                    });
                    break;
                default:
                    return BadRequest("Uknown user role");
            }

            return CreatedAtAction(nameof(Register), _mapper.Map<UserDTO>(newUser));
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
                return BadRequest("User email or password is invalid.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isPasswordValid)
                return BadRequest("User email or password is invalid.");

            var accessToken = await _tokenManager.CreateAccessTokenAsync(user);

            return Ok(accessToken);
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
            tokenRequest.Token = tokenRequest.Token.Replace("\"", string.Empty);
            tokenRequest.RefreshToken = tokenRequest.RefreshToken.Replace("\"", string.Empty);

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            var result = await _tokenManager.VerifyAndGenerateToken(tokenRequest);

            if (result == null)
                BadRequest("Invalid tokens");

            return Ok(result);
        }

        private async Task<List<Hobby>> GetHobbiesAsync(string hobbiesJsonString)
        {
            var hobbies = new List<Hobby>();

            var hobbiesNames = hobbiesJsonString.Split(',').ToList();

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
    }
}
