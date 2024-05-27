using System;

namespace IBLTermocasa.Contacts;

[Serializable]
public class ContactExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}