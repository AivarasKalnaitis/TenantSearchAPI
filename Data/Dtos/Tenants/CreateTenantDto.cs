using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Tenants
{
    public record CreateTenantDto(
        [Required]
        string Name,

        [Required]
        string Surname,

        [Required]
        Gender Gender,

        List<Hobby> Hobbies,

        List<Review> Reviews
    );
}
