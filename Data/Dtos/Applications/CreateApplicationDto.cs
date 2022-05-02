using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Applications
{
    public class CreateApplicationDto
    {
        public DateTime AppliedAt { get; set; } = DateTime.Now;
        public DateTime? SelectedAt { get; set; }
        public DateTime? ValidUntil { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.PENDING;
        
        [Required]
        public string TenantId { get; set; }

        [Required]
        public string ApartmentId { get; set; }
    }
}
