using System;

namespace IBLTermocasa.BillOfMaterials;

[Serializable]
public class BillOFMaterialExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}