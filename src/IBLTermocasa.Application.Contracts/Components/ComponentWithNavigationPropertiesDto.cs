using IBLTermocasa.Materials;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Components
{
    public abstract class ComponentWithNavigationPropertiesDtoBase
    {
        public ComponentDto Component { get; set; } = null!;

        public List<MaterialDto> Materials { get; set; } = new List<MaterialDto>();

    }
}