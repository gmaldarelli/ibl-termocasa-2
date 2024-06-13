using System;

namespace IBLTermocasa.BillOFMaterials
{
    public class BillOFMaterialExcelDto
    {
        public string Name { get; set; } = null!;
        public string RequestForQuotationId { get; set; } = null!;
        public string? ListItems { get; set; }
    }
}