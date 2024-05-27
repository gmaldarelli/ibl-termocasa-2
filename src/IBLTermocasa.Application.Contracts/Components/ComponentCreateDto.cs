using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Components
{
    public class ComponentCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}