using System;

namespace IBLTermocasa.Common;

public class ContactProperty : EntityProperty
{
    public ContactProperty()
    {
    }

    public ContactProperty(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}