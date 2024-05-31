using System;

namespace IBLTermocasa.Components
{
    public class ComponentItemDto 
    {
        public Guid Id { get; set; }
        public bool IsDefault { get; set; }
        public Guid MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }

        public string ToString()
        {
            return $"Id: {Id}, IsDefault: {IsDefault}, MaterialId: {MaterialId}, MaterialCode: {MaterialCode}, MaterialName: {MaterialName}";
        }
    }
}