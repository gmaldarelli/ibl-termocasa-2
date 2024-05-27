using System;

namespace IBLTermocasa.Products;

[Serializable]
public class ProductExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}