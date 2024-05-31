using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Components
{
    public class ComponentUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;

        public List<ComponentItemDto> ComponentItems { get; set; } = new();
        
        public string ConcurrencyStamp { get; set; } = null!;
    }
}