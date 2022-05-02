using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.DTOs.Auth
{
    public class RegisterTenantDto : RegisterUserDto
    {
        [Required]
        public string Hobbies { get; set; }
    }
}