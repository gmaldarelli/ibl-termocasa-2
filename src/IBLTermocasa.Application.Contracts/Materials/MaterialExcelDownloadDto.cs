using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Materials
{
    public class MaterialExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }
        public SourceType? SourceType { get; set; }

        public MaterialExcelDownloadDto()
        {

        }
    }
}