using IBLTermocasa.Materials;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemWithNavigationPropertiesDtoBase
    {
        public ComponentItemDto ComponentItem { get; set; } = null!;

        public MaterialDto Material { get; set; } = null!;

    }
}