using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class SubProductDto : EntityDto<Guid>
    { 
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; }
        public bool Mandatory { get; set; }
        public List<Guid> ProductIds { get; set; } = new();

    }
}