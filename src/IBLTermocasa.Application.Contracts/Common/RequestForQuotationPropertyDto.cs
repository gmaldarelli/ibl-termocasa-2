using System;

namespace IBLTermocasa.Common;

public class RequestForQuotationPropertyDto : EntityPropertyDto
{
    public RequestForQuotationPropertyDto()
    {
    }
    
    public RequestForQuotationPropertyDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}