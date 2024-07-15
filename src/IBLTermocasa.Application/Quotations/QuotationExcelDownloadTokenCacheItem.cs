using System;

namespace IBLTermocasa.Quotations;

[Serializable]
public class QuotationExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}