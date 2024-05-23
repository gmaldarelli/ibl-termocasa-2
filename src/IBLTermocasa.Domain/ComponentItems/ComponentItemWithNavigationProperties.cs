using IBLTermocasa.Materials;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemWithNavigationPropertiesBase
    {
        public ComponentItem ComponentItem { get; set; } = null!;

        public Material Material { get; set; } = null!;
        

        
    }
}