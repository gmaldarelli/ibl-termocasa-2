using System;
using System.Numerics;

namespace IBLTermocasa.Common;

public class ViewElementPropertyDto<T>(string name, T value)
{
    public string Name { get; set; } = name;

    public T Value { get; set; } = value;

    public ViewElementPropertyDto(string name) : this(name, default!)
    {
    }
    
    public ViewElementPropertyDto() : this("", default!)
    {
    }
}