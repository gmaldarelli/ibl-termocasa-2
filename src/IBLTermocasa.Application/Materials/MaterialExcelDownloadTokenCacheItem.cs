using System;

namespace IBLTermocasa.Materials;

[Serializable]
public class MaterialExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}