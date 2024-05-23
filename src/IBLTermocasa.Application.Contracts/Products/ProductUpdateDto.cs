using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public abstract class ProductUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; }
        public bool IsInternal { get; set; }
        public List<Guid> ComponentIds { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}