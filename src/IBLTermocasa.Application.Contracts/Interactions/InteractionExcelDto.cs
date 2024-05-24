using IBLTermocasa.Types;
using System;

namespace IBLTermocasa.Interactions
{
    public class InteractionExcelDto
    {
        public InteractionType InteractionType { get; set; }
        public DateTime InteractionDate { get; set; }
        public string? ReferenceObject { get; set; }
    }
}