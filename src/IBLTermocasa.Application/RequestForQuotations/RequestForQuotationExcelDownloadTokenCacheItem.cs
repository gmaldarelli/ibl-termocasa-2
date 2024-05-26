using System;

namespace IBLTermocasa.RequestForQuotations;

[Serializable]
public class RequestForQuotationExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}