using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.BillOfMaterials
{
    public class GetBillOfMaterialsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public RequestForQuotationProperty? RequestForQuotationProperty { get; set; } = new();

        public GetBillOfMaterialsInput()
        {

        }
    }
}