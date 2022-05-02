using System;
using System.Collections.Generic;
using TenantSearchAPI.Data.Entities;
using TenantSearchAPI.Data.Enums;

namespace TenantSearchAPI.Data.Dtos.Landlords
{
    public record LandlordDto(Guid Id, string Name, string Surname, Gender Gender, ICollection<Apartment> Apartments, Guid UserId);
}
