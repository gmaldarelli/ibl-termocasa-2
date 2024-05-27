using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Products
{
    public class ProductCreateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; } = false;
        public bool IsInternal { get; set; } = false;
        public List<Guid> ComponentIds { get; set; }
        public List<Guid> QuestionTemplateIds { get; set; }
    }
}