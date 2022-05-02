using System.ComponentModel.DataAnnotations;

namespace TenantSearchAPI.Data.Dtos.Hobbies
{
    public record CreateHobbyDto(
        [Required]
        string Name
    );
}
