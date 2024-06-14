using System;
using System.Numerics;

namespace IBLTermocasa.Common;

public class ViewElementPropertyDto
{
    public string Name { get; set; }

    public Object? Value { get; set; }
    public Type? Type { get; set; }
    
    public ViewElementPropertyDto(string name, Object value, Type type)
    {
        Name = name;
        Value = value;
        Type = type;
    }
    public ViewElementPropertyDto(string name, Object value)
    {
        Name = name;
        Value = value;
        if (Value == null)
        {
            Type = typeof(Object);
            return;
        }
        Type = value.GetType();
    }
    
    public ViewElementPropertyDto(string name)
    {
        Name = name;
        Value = null;
        Type = typeof(Object);
    }
    
    public ViewElementPropertyDto()
    {
        Name = "";
        Value = null;
        Type = typeof(Object);
    }
    
    public string ValueToString()
    {
        if (Value == null)
        {
            return "";
        }

        switch (Type!.Name)
        {
            case "string":
            {
                return Value.ToString();
            }
            case "int":
            {
                return Value.ToString();
            }
             default:
                return Value.ToString();
        }
    }
}