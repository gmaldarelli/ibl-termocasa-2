using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class SubProductDto : EntityDto<Guid>
    { 
        [Required]
        public int Order { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; }
        public bool Mandatory { get; set; }
        public List<Guid> ProductIds { get; set; } = new();

    }
}