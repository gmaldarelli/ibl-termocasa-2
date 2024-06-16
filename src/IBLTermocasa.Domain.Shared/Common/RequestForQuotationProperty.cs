using System;

namespace IBLTermocasa.Common;

public class RequestForQuotationProperty : EntityProperty
{
    public string? OrganizationName { get; set; }
    public DateTime? RfqDateDocument { get; set; }
    public string? RfqNumber { get; set; }
    public RequestForQuotationProperty()
    {
    }

    public RequestForQuotationProperty(Guid id, string? name, string? organizationName, DateTime? rfqDateDocument, string? rfqNumber)
    {
        Id = id;
        Name = name;
        OrganizationName = organizationName;
        RfqDateDocument = rfqDateDocument;
        RfqNumber = rfqNumber;
    }
}