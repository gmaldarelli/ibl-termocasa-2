using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Subproducts
{
    public class SubproductCreateDto
    {
        public Guid ProductId { get; set; }
        public int Order { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; } = true;
        public bool Mandatory { get; set; }
        public Guid? SingleProductId { get; set; }
    }
}