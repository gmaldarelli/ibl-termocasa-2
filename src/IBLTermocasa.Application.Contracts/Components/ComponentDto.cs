using System;
using System.Collections.Generic;
using IBLTermocasa.ComponentItems;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Components
{
    public abstract class ComponentDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

        public List<ComponentItemWithNavigationPropertiesDto> ComponentItems { get; set; } = new();
    }
}