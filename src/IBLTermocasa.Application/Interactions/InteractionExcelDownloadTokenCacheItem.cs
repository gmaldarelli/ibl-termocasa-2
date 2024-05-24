using System;

namespace IBLTermocasa.Interactions;

[Serializable]
public class InteractionExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}