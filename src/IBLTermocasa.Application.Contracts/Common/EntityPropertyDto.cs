using System;

namespace IBLTermocasa.Common;

public abstract class EntityPropertyDto
{
    public virtual Guid Id { get; set; }
    public virtual string? Name { get; set; }
}