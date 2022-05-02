using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Applications
{
    public record ApplicationDto(Guid Id, DateTime AppliedAt, DateTime SelectedAt, DateTime ValidUntil, ApplicationStatus Status, Guid TenantId, Guid ApartmentId);
}
