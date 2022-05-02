using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TenantSearchAPI.Data.Dtos.Reviews
{
    public record UpdateReviewDto(
        [Required]
        string Content
    );
}