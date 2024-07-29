using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using IBLTermocasa.Types;

namespace IBLTermocasa.BillOfMaterials
{
    public class GetBillOfMaterialsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? BomNumber { get; set; }
        public RequestForQuotationProperty? RequestForQuotationProperty { get; set; } = new();
        public BomStatusType? Status { get; set; }
        
        public GetBillOfMaterialsInput()
        {
            
        }
    }
}