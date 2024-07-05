using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? ConsumptionProduct { get; set; }
        public string? ConsumptionWork { get; set; }

        public ConsumptionEstimationExcelDownloadDto()
        {

        }
    }
}