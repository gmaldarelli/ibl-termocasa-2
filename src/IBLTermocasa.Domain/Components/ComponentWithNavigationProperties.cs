using IBLTermocasa.Materials;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Components
{
    public class ComponentWithNavigationProperties
    {
        public Component Component { get; set; } = null!;

        

        public List<Material> Materials { get; set; } = null!;
        
    }
}