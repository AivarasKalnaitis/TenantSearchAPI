using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantSearchAPI.Auth.Model
{
    public class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string Landlord = nameof(Landlord);
        public const string Tenant = nameof(Tenant);
        public const string TenantOrLandlord = nameof(Tenant) + "," + nameof(Landlord);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, Landlord, Tenant };
    }
}
