using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Components
{
    public abstract class ComponentExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public ComponentExcelDownloadDtoBase()
        {

        }
    }
}