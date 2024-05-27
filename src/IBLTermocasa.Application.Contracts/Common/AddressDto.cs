using System.Text;

namespace IBLTermocasa.Common;

public class AddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? AdditionalInfo { get; set; }
    
    public string ResolveAddress()
    {
        StringBuilder sb = new StringBuilder();
        string elements = "";
        elements = Street is null ? "" : $"{Street}, ";
        sb.Append(elements);
        elements = PostalCode is null ? "" : $"{PostalCode}, ";
        sb.Append(elements);
        elements = City is null ? "" : $"{City}, ";
        sb.Append(elements);
        elements = State is null ? "" : $"{State}, ";
        sb.Append(elements);
        elements = Country is null ? "" : $"{Country}, ";
        sb.Append(elements);
        return sb.ToString();
    }
}