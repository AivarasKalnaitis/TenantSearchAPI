using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Hobbies
{
    public record UpdateHobbyDto(
        [Required]
        string Name
    );
}
