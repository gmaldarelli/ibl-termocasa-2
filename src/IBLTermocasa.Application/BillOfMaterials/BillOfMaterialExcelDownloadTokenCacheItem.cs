using System;

namespace IBLTermocasa.BillOfMaterials;

[Serializable]
public class BillOfMaterialExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}