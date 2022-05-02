using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Entities
{
    public class Apartment
    {
        public Guid Id { get; set; }
        public ApartmentType Type { get; set; }
        public double Price { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int Rooms { get; set; }
        public double Area { get; set; }
        public Guid LandlordId { get; set; }
        public Guid TenantId { get; set; }
    }
}
