using System.Collections.Generic;

namespace IBLTermocasa.Common;

public class PhoneInfoDto
{
    public List<PhoneItemDto> PhoneItems { get; set; } = new();
}

public class PhoneItemDto
{
    public PhoneType Type { get; set; }
    public string? Prefix { get; set; }
    public string Number { get; set; }
    public override string ToString()
    {
        return $"{Prefix} {Number}";
    }}
