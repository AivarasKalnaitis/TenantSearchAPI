using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TenantSearchAPI.Auth.Model;
using TenantSearchAPI.Data.DTOs.Auth;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Entities
{
    public class Landlord : IUserOwnedResource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public ICollection<Apartment> Apartments { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
