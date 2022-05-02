using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.Dtos.Reviews;
using TenantSearchAPI.Data.Dtos.Tenants;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Repositories;

namespace TenantSearchAPI.Controllers
{
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReviewsRepository _reviewsRepository;
        private readonly ITenantsRepository _tenantsRepository;
        private readonly ILandlordsRepository _landlordsRepository;
        private readonly IAuthorizationService _authorizationService;

        public ReviewController(IMapper mapper, IReviewsRepository reviewsRepository, ITenantsRepository tenantsRepository, ILandlordsRepository landlordsRepository, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _reviewsRepository = reviewsRepository;
            _tenantsRepository = tenantsRepository;
            _landlordsRepository = landlordsRepository;
            _authorizationService = authorizationService;
        }

        [Route("api/reviews")]
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IEnumerable<ReviewDto>> GetAllFromAllTenants(Guid tenantId)
        {
            return (await _reviewsRepository.GetAllFromAllTenants()).Select(o => _mapper.Map<ReviewDto>(o));
        }

        [Route("api/tenants/{tenantId}/reviews")]
        [HttpGet]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<IEnumerable<ReviewDto>> GetAll(Guid tenantId)
        {
            return (await _reviewsRepository.GetAll(tenantId)).Select(o => _mapper.Map<ReviewDto>(o));
        }

        [Route("api/tenants/{tenantId}/reviews")]
        [HttpGet("{reviewId}")]
        [Authorize(Roles = UserRoles.TenantOrLandlord)]
        public async Task<ActionResult<ReviewDto>> GetById(Guid reviewId)
        {
            var review = await _reviewsRepository.GetById(reviewId);

            if (review == null)
                return NotFound($"Review with id {reviewId} does not exist");

            return Ok(_mapper.Map<ReviewDto>(review));
        }

        [Route("api/tenants/{tenantId}/reviews")]
        [HttpPost]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult<ReviewDto>> Create(Guid tenantId, CreateReviewDto reviewDto)
        {
            var review = new Review();
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant != null)
            {
                review.TenantId = tenantId;
                review.Content = reviewDto.Content;
                review.LandlordId = reviewDto.LandlordId;
                tenant.Reviews.Add(review);
                await _tenantsRepository.Update(tenant);

                return Created($"/api/tenants/{tenant.Id}", _mapper.Map<ReviewDto>(review));
            }

            return BadRequest($"Tenant with id {tenantId} does not exist.");
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Landlord)]
        [Route("api/tenants/{tenantId}/reviews/{reviewId}")]
        public async Task<ActionResult<TenantDto>> Update(Guid tenantId, Guid reviewId, UpdateReviewDto reviewDto)
        {
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant == null)
                return NotFound($"Tenant with id {tenantId} does not exist");

            var review = await _reviewsRepository.GetById(reviewId);

            if (review == null)
                return NotFound($"Review with id {reviewId} does not exist");

            var landlord = await _landlordsRepository.GetById(review.LandlordId);

            if (landlord == null)
                return NotFound($"Landlord with id {landlord.Id} does not exist");

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, landlord, PolicyNames.SameUser);

            if (!authorizationResult.Succeeded)
                return Forbid();

            _mapper.Map(reviewDto, review);

            await _reviewsRepository.Update(review);

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [Route("api/tenants/{tenantId}/reviews/{reviewId}")]
        [HttpDelete]
        [Authorize(Roles = UserRoles.Landlord)]
        public async Task<ActionResult> Delete(Guid tenantId, Guid reviewId)
        {
            var review = await _reviewsRepository.GetById(reviewId);
            var tenant = await _tenantsRepository.GetById(tenantId);

            if (tenant != null)
            {

                if (review == null)
                    return NotFound($"Review with id \'{reviewId}' does not exist.");

                var landlord = await _landlordsRepository.GetById(review.LandlordId);

                if (landlord == null)
                    return NotFound($"Landlord with id {landlord.Id} does not exist");

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, landlord, PolicyNames.SameUser);

                if (!authorizationResult.Succeeded)
                    return Forbid();

                await _reviewsRepository.Delete(review);

                return NoContent();
            }

            return BadRequest($"Tenant with id \'{tenantId}\' does not exist.");
        }
    }
}
