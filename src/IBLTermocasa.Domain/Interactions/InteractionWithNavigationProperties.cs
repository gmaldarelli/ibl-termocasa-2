using Volo.Abp.Identity;
using Volo.Abp.Identity;
using Volo.Abp.Identity;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Interactions
{
    public  class InteractionWithNavigationProperties
    {
        public Interaction Interaction { get; set; } = null!;

        public IdentityUser IdentityUser { get; set; } = null!;
        public OrganizationUnit OrganizationUnit { get; set; } = null!;
        public IdentityUser IdentityUser1 { get; set; } = null!;
        

        
    }
}