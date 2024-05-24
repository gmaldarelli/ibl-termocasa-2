using IBLTermocasa.Industries;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Organizations
{
    public  class OrganizationWithNavigationProperties
    {
        public Organization Organization { get; set; } = null!;

        public Industry Industry { get; set; } = null!;
        

        
    }
}