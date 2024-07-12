using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfileExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }
        public string? Code { get; set; }

        public string? Name { get; set; }
        public double? StandardPriceMin { get; set; }
        public double? StandardPriceMax { get; set; }

        public ProfessionalProfileExcelDownloadDto()
        {

        }
    }
}