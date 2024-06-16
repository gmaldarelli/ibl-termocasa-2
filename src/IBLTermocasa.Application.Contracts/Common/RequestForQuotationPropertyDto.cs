using System;

namespace IBLTermocasa.Common;

public class RequestForQuotationPropertyDto : EntityPropertyDto
{
    public virtual string? OrganizationName { get; set; }
    public virtual DateTime? RfqDateDocument { get; set; }
    public virtual string RfqNumber { get; set; }
    public RequestForQuotationPropertyDto()
    {
    }
    
    public RequestForQuotationPropertyDto(Guid id, string? name, string? organizationName, DateTime? rfqDateDocument, string? rfqNumber)
    {
        Id = id;
        Name = name;
        OrganizationName = organizationName;
        RfqDateDocument = rfqDateDocument;
        RfqNumber = rfqNumber;
    }   
}