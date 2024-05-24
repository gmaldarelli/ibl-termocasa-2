using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Industries
{
    public class IndustryCreateDto
    {
        [Required]
        [StringLength(IndustryConsts.CodeMaxLength)]
        public string Code { get; set; } = null!;
        [StringLength(IndustryConsts.DescriptionMaxLength)]
        public string? Description { get; set; }
    }
}