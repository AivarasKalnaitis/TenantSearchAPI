using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Tenants
{
    public record UpdateTenantDto(
        [Required]
        string Name,

        [Required]
        string Surname,

        [Required]
        int Gender,

        List<string> Hobbies
    );
}