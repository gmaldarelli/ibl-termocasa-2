using System;

namespace IBLTermocasa.Types;

public enum OrganizationType
{
    [ContextPath("leads")]
    LEAD = 1,
    [ContextPath("customers")]
    CUSTOMER = 2,
    [ContextPath("partners")]
    PARTNER = 3,
    [ContextPath("suppliers")]
    SUPPLIER = 4,
    [ContextPath("competitors")]
    COMPETITOR = 5
}
public class ContextPathAttribute : Attribute
{
    public string ContextPath { get; }

    public ContextPathAttribute(string contextPath)
    {
        ContextPath = contextPath;
    }
}
    