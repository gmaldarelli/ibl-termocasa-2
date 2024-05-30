using IBLTermocasa.Materials;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Components
{
    public class ComponentItem
    {
        
        public Guid Id { get; set; }
        public bool IsDefault { get; set; }
        public Guid MaterialId { get; set; }

        protected ComponentItem()
        {
            Id = Guid.NewGuid();
        }

        public ComponentItem(Guid id, Guid materialId, bool isDefault)
        {
            Id = id;
            IsDefault = isDefault;
            MaterialId = materialId;
        }

    }
}