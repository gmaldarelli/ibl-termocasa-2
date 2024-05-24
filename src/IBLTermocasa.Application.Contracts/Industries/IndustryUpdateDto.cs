using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Industries
{
    public class IndustryUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(IndustryConsts.CodeMaxLength)]
        public string Code { get; set; } = null!;
        [StringLength(IndustryConsts.DescriptionMaxLength)]
        public string? Description { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}