using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Applications
{
    public class UpdateApplicationDto
    {
        public string Status { get; set; }
        public string Months { get; set; }
    }
}
