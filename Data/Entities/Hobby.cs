using System;
using System.Collections.Generic;

namespace TenantSearchAPI.Data.Entities
{
    public class Hobby
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Tenant> Tenants { get; set; }
    }
}