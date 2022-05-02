using Microsoft.AspNetCore.Identity;

namespace TenantSearchAPI.Data.DTOs.Auth
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string AdditionalInfo { get; set; }
    }
}
