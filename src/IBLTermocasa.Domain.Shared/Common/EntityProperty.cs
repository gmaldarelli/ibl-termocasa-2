using System;

namespace IBLTermocasa.Common;

public abstract class EntityProperty
{
    public virtual Guid Id { get; set; }
    public virtual string? Name { get; set; }
}