using System;

namespace IBLTermocasa.Products
{
    public abstract class ProductExcelDtoBase
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsAssembled { get; set; }
        public bool IsInternal { get; set; }
    }
}