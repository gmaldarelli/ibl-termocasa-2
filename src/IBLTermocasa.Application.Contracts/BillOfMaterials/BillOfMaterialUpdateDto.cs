﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOfMaterialUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string BomNumber { get; set; } = null!;
        [Required]
        public RequestForQuotationPropertyDto RequestForQuotationProperty { get; set; } = null!;
        public List<BomItemDto>? ListItems { get; set; } = new();

        public string ConcurrencyStamp { get; set; } = null!;
    }
}