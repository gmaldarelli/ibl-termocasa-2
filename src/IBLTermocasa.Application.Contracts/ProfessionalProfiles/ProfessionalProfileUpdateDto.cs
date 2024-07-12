using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfileUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public double StandardPrice { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}