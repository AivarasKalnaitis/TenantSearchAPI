using AutoMapper;
using TenantSearchAPI.Data.Dtos.Apartments;
using TenantSearchAPI.Data.Dtos.Applications;
using TenantSearchAPI.Data.Dtos.Hobbies;
using TenantSearchAPI.Data.Dtos.Landlords;
using TenantSearchAPI.Data.Dtos.Reviews;
using TenantSearchAPI.Data.Dtos.Tenants;
using TenantSearchAPI.Data.DTOs.Auth;
using TenantSearchAPI.Data.Entities;

namespace TenantSearchAPI.Data
{
    public class TenantSearchProfile : Profile
    {
        public TenantSearchProfile()
        {
            CreateMap<Tenant, TenantDto>();
            CreateMap<CreateTenantDto, Tenant>();
            CreateMap<UpdateTenantDto, Tenant>();

            CreateMap<Hobby, HobbyDto>();
            CreateMap<CreateHobbyDto, Hobby>();
            CreateMap<UpdateHobbyDto, Hobby>();

            CreateMap<Apartment, ApartmentDto>();
            CreateMap<CreateApartmentDto, Apartment>();
            CreateMap<UpdateApartmentDto, Apartment>();

            CreateMap<Landlord, LandlordDto>();
            CreateMap<CreateLandlordDto, Landlord>();
            CreateMap<UpdateLandlordDto, Landlord>();

            CreateMap<Review, ReviewDto>();
            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();

            CreateMap<User, UserDTO>();

            CreateMap<Application, ApplicationDto>();
            CreateMap<CreateApplicationDto, Application>();
            CreateMap<UpdateApplicationDto, Application>();
        }
    }
}
