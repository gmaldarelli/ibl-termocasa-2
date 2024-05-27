using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Catalogs
{
    public class CatalogUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Name { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Description { get; set; }
        public List<Guid> ProductIds { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}