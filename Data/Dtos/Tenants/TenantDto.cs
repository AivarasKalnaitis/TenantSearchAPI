using System;
using System.Collections.Generic;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Tenants
{
    public record TenantDto(
        Guid Id, 
        string Name, 
        string Surname, 
        Gender Gender, 
        List<Hobby> Hobbies, 
        List<Review> Reviews, 
        Guid UserId
    );
}
