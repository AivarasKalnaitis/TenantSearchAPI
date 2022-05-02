using System;

namespace TenantSearch.Objects
{
    public class TenantTagsCount
    {
        public Guid TenantId { get; set; }

        public double[] TagCounts { get; set; }

        public TenantTagsCount(Guid tenantId, int numTags)
        {
            TenantId = tenantId;
            TagCounts = new double[numTags];
        }
    }
}