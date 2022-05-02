using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantSearchAPI.Auth.Model
{
    public interface IUserOwnedResource
    {
        Guid UserId { get; }
    }
}
