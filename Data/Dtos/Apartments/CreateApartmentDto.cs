using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Apartments
{
    public record CreateApartmentDto(

        [Required]
        string Type,

        [Required]
        decimal Price,

        [Required]
        string City,

        [Required]
        string Address,

        [Required]
        int Rooms,

        [Required]
        decimal Area,

        [Required]
        string landlordId
    );
}
