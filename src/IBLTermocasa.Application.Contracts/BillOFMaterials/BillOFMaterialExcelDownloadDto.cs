using Volo.Abp.Application.Dtos;
using System;
using IBLTermocasa.Common;

namespace IBLTermocasa.BillOFMaterials
{
    public class BillOFMaterialExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public RequestForQuotationProperty? RequestForQuotationId { get; set; }

        public BillOFMaterialExcelDownloadDto()
        {

        }
    }
}