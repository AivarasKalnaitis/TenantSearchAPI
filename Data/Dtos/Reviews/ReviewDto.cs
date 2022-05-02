using System;
using System.ComponentModel.DataAnnotations;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Reviews
{
    public record ReviewDto(
        [Required]
        Guid Id,

        [Required]
        string Content,

        [Required]
        Guid LandlordId,

        [Required]
        Guid TenantId
    );
}
