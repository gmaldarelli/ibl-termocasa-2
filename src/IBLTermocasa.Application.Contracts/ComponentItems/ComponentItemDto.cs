using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemDtoBase : EntityDto<Guid>
    {
        public Guid ComponentId { get; set; }
        public bool IsDefault { get; set; }
        public Guid MaterialId { get; set; }

    }
}