using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Components
{
    public class ComponentExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public ComponentExcelDownloadDto()
        {

        }
    }
}