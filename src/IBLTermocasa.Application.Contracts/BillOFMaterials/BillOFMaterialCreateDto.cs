﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.BillOFMaterials
{
    public class BillOFMaterialCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public RequestForQuotationPropertyDto RequestForQuotationProperty { get; set; }
        public List<BOMItemDto>? ListItems { get; set; } = new();
    }
}