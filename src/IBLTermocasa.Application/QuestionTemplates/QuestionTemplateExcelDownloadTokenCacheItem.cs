using System;

namespace IBLTermocasa.QuestionTemplates;

[Serializable]
public class QuestionTemplateExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}