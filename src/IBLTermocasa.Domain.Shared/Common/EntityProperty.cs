using System;

namespace IBLTermocasa.Common;

public abstract class EntityProperty
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}