using System;

namespace IBLTermocasa.Catalogs;

[Serializable]
public class CatalogExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}