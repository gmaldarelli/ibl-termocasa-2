using System;

namespace IBLTermocasa.Common;

public class AgentProperty : EntityProperty
{
    public AgentProperty()
    {
    }
    public AgentProperty(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}