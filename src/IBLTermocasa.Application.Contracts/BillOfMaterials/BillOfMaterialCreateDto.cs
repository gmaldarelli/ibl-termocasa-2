using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOfMaterialCreateDto
    {
        [Required]
        public string BomNumber { get; set; }
        [Required]
        public RequestForQuotationPropertyDto RequestForQuotationProperty { get; set; }
        public List<BomItemDto>? ListItems { get; set; } = new();
    }
}