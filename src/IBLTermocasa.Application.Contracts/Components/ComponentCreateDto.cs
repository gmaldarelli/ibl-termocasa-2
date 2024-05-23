using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Components
{
    public abstract class ComponentCreateDtoBase
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}