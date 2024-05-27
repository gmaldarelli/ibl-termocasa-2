using System.Collections.Generic;

namespace IBLTermocasa.Common;

public class PhoneInfo
{
    public List<PhoneItem> PhoneItems { get; set; } = new();
}

public class PhoneItem
{
    public PhoneType Type { get; set; }
    public string? Prefix { get; set; }
    public string Number { get; set; }
}

public enum PhoneType
{
    PHONE_HOME,
    PHONE_WORK,
    MOBILE_HOME,
    MOBILE_WORK,
    FAX,
    OTHER
}