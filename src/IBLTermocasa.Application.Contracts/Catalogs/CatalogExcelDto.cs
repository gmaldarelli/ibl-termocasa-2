using System;

namespace IBLTermocasa.Catalogs
{
    public class CatalogExcelDto
    {
        public string Name { get; set; } = null!;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Description { get; set; }
    }
}