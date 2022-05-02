namespace TenantSearch.Objects
{
    public class TenantTag
    {
        public string TenantId { get; set; }

        public string TagName { get; set; }

        public TenantTag(string tenantId, string tag)
        {
            TenantId = tenantId;
            TagName = tag;
        }
    }
}
