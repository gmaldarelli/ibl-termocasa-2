using System;

namespace IBLTermocasa.Organizations;

[Serializable]
public class OrganizationExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}