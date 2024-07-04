using System;

namespace IBLTermocasa.ConsumptionEstimations;

[Serializable]
public class ConsumptionEstimationExcelDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}