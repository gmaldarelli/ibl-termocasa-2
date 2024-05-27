using System;

namespace IBLTermocasa.Common;

public class OrganizationProperty : EntityProperty
{
    public OrganizationProperty()
    {
    }
    public OrganizationProperty(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}