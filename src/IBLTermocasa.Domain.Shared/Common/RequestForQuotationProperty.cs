using System;

namespace IBLTermocasa.Common;

public class RequestForQuotationProperty : EntityProperty
{
    public RequestForQuotationProperty()
    {
    }

    public RequestForQuotationProperty(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}