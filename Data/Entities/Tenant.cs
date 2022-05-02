using System;
using System.Collections.Generic;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public List<Hobby> Hobbies { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid UserId { get; set; }
    }
}
