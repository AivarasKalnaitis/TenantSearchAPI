using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearch.Objects;
using TenantSearchAPI.Data.Dtos.Landlords;
using TenantSearchAPI.Data.Dtos.Tenants;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;
using TenantSearchAPI.Data.Repositories;
using TenantSearchAPI.RecommendationService.Objects;

namespace TenantSearch
{
    [Serializable]
    public class UserBehaviorDatabase
    {
        public static List<Tag> TenantTags { get; set; }

        public IEnumerable<TenantDto> Tenants { get; set; }

        public IEnumerable<LandlordDto> Landlords { get; set; }

        public List<LandlordHistory> LandlordsHistory { get; set; }

        private readonly ITenantsRepository _tenantsRepository;
        private readonly IApartmentsRepository _apartmentsRepository;
        private readonly ILandlordsRepository _landlordsRepository;
        private readonly IApplicationsRepository _applicationsRepository;
        private readonly IHobbiesRepository _hobbiesRepository;
        private readonly IMapper _mappper;

        public UserBehaviorDatabase(ITenantsRepository tenantsRepository, ILandlordsRepository landlordsRepository, IMapper mapper, IApartmentsRepository apartmentsRepository, IApplicationsRepository applicationsRepository, IHobbiesRepository hobbiesRepository)
        {
            _mappper = mapper;
            _tenantsRepository = tenantsRepository;
            _landlordsRepository = landlordsRepository;
            _apartmentsRepository = apartmentsRepository;
            _applicationsRepository = applicationsRepository;
            _hobbiesRepository = hobbiesRepository;
        }

        public async Task RetrieveAllData()
        {
            TenantTags = await GetTags();
            Tenants = (await _tenantsRepository.GetAll()).Select(o => _mappper.Map<TenantDto>(o));
            Landlords = (await _landlordsRepository.GetAll()).Select(o => _mappper.Map<LandlordDto>(o));

            var landlordsIds = Landlords.Select(o => o.Id).ToList();
            LandlordsHistory = await GetHistories(landlordsIds);
        }

        private async Task<List<LandlordHistory>> GetHistories(List<Guid> landlordsIds)
        {
            var histories = new List<LandlordHistory>();

            foreach (var landlordId in landlordsIds)
            {
                var landlord = await _landlordsRepository.GetById(landlordId);

                if (landlord == null)
                {
                    histories.Add(new LandlordHistory(landlordId));
                    break;
                }

                var apartmentsIds = (await _apartmentsRepository.GetByLandlord(landlordId)).Select(a => a.Id);
                var applications = await _applicationsRepository.GetAll();
                var landlordApplications = new List<Application>();

                foreach (var appartmentId in apartmentsIds)
                {
                    var apartmentApplications = applications.Where(x => x.ApartmentId == appartmentId);

                    if (apartmentApplications.Any())
                    {
                        landlordApplications.AddRange(apartmentApplications);
                    }
                }


                histories.Add(new LandlordHistory(landlordId, landlordApplications));
            }

            return histories;
        }

        private async Task<List<Tag>> GetTags()
        {
            var hobbies = (await _hobbiesRepository.GetAll()).Select(x => x.Name.ToUpper()).ToList();
            var genders = new List<string>();
            
            foreach(Gender gender in Enum.GetValues(typeof(Gender)))
            {
                genders.Add(Enum.GetName(gender));
            }

            var tags = new List<Tag>();
            tags.AddRange(hobbies.Select(o => new Tag(o)));
            tags.AddRange(genders.Select(o => new Tag(o)));

            return tags;
        }

        //public List<TenantTag> GetTenantTagLinkingTable()
        //{
        //    List<TenantTag> tenantTags = new();

        //    foreach (TenantDto tenant in Tenants)
        //    {
        //        foreach (Tag tag in tenant.Tags)
        //        {
        //            tenantTags.Add(new TenantTag(tenant.TenantId, tag.Name));
        //        }
        //    }

        //    return tenantTags;
        //}

        //public List<Tag> GetAllTags()
        //{
        //    return TenantTags;
        //}
    }
}
