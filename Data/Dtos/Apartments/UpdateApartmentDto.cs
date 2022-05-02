using System.ComponentModel.DataAnnotations;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Apartments
{
    public record UpdateApartmentDto(
        [Required]
        string id,

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
        string landlordId,

        string tenantId
    );
}
