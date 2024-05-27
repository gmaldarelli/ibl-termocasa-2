using System;

namespace IBLTermocasa.Common;

public class OrganizationPropertyDto : EntityPropertyDto
{
    public OrganizationPropertyDto()
    {
    }
    
    public OrganizationPropertyDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}