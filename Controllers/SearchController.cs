using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Apartments;
using TenantSearchAPI.Data.Dtos.Tenants;
using TenantSearchAPI.Data.Repositories;

namespace RelicsAPI.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IApartmentsRepository _apartmentsRepository;
        private readonly IMapper _mapper;

        public SearchController(ITenantsRepository tenantsRepository, IApartmentsRepository apartmentsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _apartmentsRepository = apartmentsRepository;
            _mapper = mapper;
        }

        [Route("tenants")]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<IEnumerable<TenantDto>> SearchTenants(string query)
        {
            var tenants = await _tenantsRepository.GetAll();

            var filteredTenants = tenants.Where(r => r.Name.ToLower().Contains(query) 
                                    || r.Surname.ToLower().Contains(query) 
                                    || r.Name + ' ' + r.Surname == query
                                    || (r.Hobbies.Any() && r.Hobbies.Where(a => a.Name.ToLower() == query.ToLower()).Any()));

            return filteredTenants.Select(r => _mapper.Map<TenantDto>(r));
        }

        [Route("apartments")]
        [Authorize(Roles = UserRoles.Tenant)]
        public async Task<IEnumerable<ApartmentDto>> SearchApartments(string query)
        {
            var apartments = await _apartmentsRepository.GetAll();

            var filteredApartments = apartments.Where(a => a.Address.ToLower().Contains(query.ToLower())
                                        || a.City.ToLower().Contains(query.ToLower()));

            return filteredApartments.Select(r => _mapper.Map<ApartmentDto>(r));
        }
    }
}
