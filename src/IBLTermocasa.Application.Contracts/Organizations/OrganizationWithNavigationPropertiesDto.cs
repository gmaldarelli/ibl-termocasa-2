using IBLTermocasa.Industries;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Organizations
{
    public class OrganizationWithNavigationPropertiesDto
    {
        public OrganizationDto Organization { get; set; } = null!;

        public IndustryDto Industry { get; set; } = null!;

    }
}