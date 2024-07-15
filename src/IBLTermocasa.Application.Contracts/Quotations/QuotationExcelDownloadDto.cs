using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Quotations
{
    public class QuotationExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? IdRFQ { get; set; }
        public string? IdBOM { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTime? SentDateMin { get; set; }
        public DateTime? SentDateMax { get; set; }
        public DateTime? QuotationValidDateMin { get; set; }
        public DateTime? QuotationValidDateMax { get; set; }
        public DateTime? ConfirmedDateMin { get; set; }
        public DateTime? ConfirmedDateMax { get; set; }
        public QuotationStatus? Status { get; set; }
        public bool? DepositRequired { get; set; }
        public double? DepositRequiredValueMin { get; set; }
        public double? DepositRequiredValueMax { get; set; }
        public string? QuotationItems { get; set; }

        public QuotationExcelDownloadDto()
        {

        }
    }
}