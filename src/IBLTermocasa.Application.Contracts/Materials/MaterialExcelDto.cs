using IBLTermocasa.Types;
using System;

namespace IBLTermocasa.Materials
{
    public class MaterialExcelDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public MeasureUnit MeasureUnit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Lifo { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal LastPrice { get; set; }
    }
}