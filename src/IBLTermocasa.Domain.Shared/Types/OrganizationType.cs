using System;

namespace IBLTermocasa.Types;

public enum OrganizationType
{
    [ContextPath("leads")]
    LEAD,
    [ContextPath("customers")]
    CUSTOMER,
    [ContextPath("partners")]
    PARTNER,
    [ContextPath("suppliers")]
    SUPPLIER,
    [ContextPath("competitors")]
    COMPETITOR
}
public class ContextPathAttribute : Attribute
{
    public string ContextPath { get; }

    public ContextPathAttribute(string contextPath)
    {
        ContextPath = contextPath;
    }
}
    