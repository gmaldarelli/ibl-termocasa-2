using IBLTermocasa.Materials;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Components
{
    public abstract class ComponentWithNavigationPropertiesBase
    {
        public Component Component { get; set; } = null!;

        

        public List<Material> Materials { get; set; } = null!;
        
    }
}