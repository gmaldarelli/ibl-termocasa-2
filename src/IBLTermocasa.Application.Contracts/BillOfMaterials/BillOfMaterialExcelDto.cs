using System;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOfMaterialExcelDto
    {
        public string BomNumber { get; set; } = null!;
        public string RequestForQuotationId { get; set; } = null!;
        public string? ListItems { get; set; }
    }
}