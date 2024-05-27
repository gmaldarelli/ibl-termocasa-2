using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Catalogs
{
    public class CatalogCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Description { get; set; }
        public List<Guid> ProductIds { get; set; }
    }
}