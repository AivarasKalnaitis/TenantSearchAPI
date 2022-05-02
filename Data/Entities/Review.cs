using System;

namespace TenantSearchAPI.Data.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid TenantId { get; set; }
        public Guid LandlordId { get; set; }
    }
}
