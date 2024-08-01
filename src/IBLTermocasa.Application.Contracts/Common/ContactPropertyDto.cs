using System;

namespace IBLTermocasa.Common;

public class ContactPropertyDto : EntityProperty
{
    public ContactPropertyDto()
    {
    }
    
    public ContactPropertyDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public override string? ToString()
    {
        return Name;
    }
}