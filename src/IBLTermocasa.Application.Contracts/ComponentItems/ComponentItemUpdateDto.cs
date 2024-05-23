using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemUpdateDtoBase
    {
        public Guid ComponentId { get; set; }
        public bool IsDefault { get; set; }
        public Guid MaterialId { get; set; }

    }
}