using System;
using System.Collections.Generic;

namespace IBLTermocasa.Common;

public class PlaceHolderUtils
{
    public static  string GetPlaceHolder(PlaceHolderType prefix, string code, string? parentPlaceHolder = null)
    {
        //TOFIX remove whitespace from parentPlaceHolder
        if (parentPlaceHolder is { Length: > 0 })
        {
            return $"{parentPlaceHolder}.{prefix.GetPrefix()} [{code}]";
        }
        return $"{prefix.GetPrefix()} [{code}]";
    }
}
public enum PlaceHolderType
{
    [Prefix("P")]
    PRODUCT,
    [Prefix("C")]
    PRODUCT_COMPONENT,
    [Prefix("Q")]
    PRODUCT_QUESTION_TEMPLATE
}

[AttributeUsage(AttributeTargets.Field)]
public class PrefixAttribute : Attribute
{
    public string Value { get; }

    public PrefixAttribute(string value)
    {
        Value = value;
    }
}


public static class EnumExtensions
{
    public static string GetPrefix(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(PrefixAttribute), false);
            if (attrs.Length > 0)
            {
                return ((PrefixAttribute)attrs[0]).Value;
            }
        }
        return "#"; 
    }
}

