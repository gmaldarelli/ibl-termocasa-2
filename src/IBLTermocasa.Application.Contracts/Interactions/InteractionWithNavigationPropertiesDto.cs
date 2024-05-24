using Volo.Abp.Identity;
using Volo.Abp.Identity;
using Volo.Abp.Identity;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Interactions
{
    public class InteractionWithNavigationPropertiesDto
    {
        public InteractionDto Interaction { get; set; } = null!;

        public IdentityUserDto IdentityUser { get; set; } = null!;
        public OrganizationUnitDto OrganizationUnit { get; set; } = null!;
        public IdentityUserDto IdentityUser1 { get; set; } = null!;

    }
}