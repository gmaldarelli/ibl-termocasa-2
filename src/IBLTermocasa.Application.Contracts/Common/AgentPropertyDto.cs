using System;

namespace IBLTermocasa.Common;

public class AgentPropertyDto : EntityPropertyDto
{
    public AgentPropertyDto()
    {
    }
    
    public AgentPropertyDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}