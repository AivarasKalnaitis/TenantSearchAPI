using System;
using System.ComponentModel.DataAnnotations;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Apartments
{
    public record ApartmentDto(
        [Required]
        Guid Id,

        [Required]
        ApartmentType Type,

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
        Guid landlordId
    );
}
