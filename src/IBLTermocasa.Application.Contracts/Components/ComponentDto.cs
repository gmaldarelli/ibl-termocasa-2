using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Components
{
    public class ComponentDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

        public List<ComponentItemDto> ComponentItems { get; set; } = new List<ComponentItemDto>();
        
        public string ToString()
        {
            return $"Id: {Id}, Name: {Name}, ConcurrencyStamp: {ConcurrencyStamp}, ComponentItems count: {ComponentItems.Count} ComponentItems: {ComponentItems.ToString()}";
        }
    }
}