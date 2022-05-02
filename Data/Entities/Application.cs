using System;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public DateTime? AppliedAt { get; set; } // jei null, reiskias dar pending application
        public DateTime? SelectedAt { get; set; }
        public DateTime? ValidUntil { get; set; } // pagal tai rodyti apartment history for Tenant pagal expired date,
                                                  // o for Landlord rodyti tenants, kurios turejo jo aprtment tam tikru laiku
        public ApplicationStatus Status {get;set;}
        public Guid TenantId { get; set; }
        public Guid ApartmentId { get; set; }
    }
}
