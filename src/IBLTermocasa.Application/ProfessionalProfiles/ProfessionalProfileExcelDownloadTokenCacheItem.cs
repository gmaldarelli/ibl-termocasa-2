using System;

namespace IBLTermocasa.ProfessionalProfiles;

[Serializable]
public class ProfessionalProfileExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}