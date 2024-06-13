using System;

namespace IBLTermocasa.BillOFMaterials;

[Serializable]
public class BillOFMaterialExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}