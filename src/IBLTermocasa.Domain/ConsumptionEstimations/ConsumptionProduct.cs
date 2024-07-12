using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionProduct : Entity<Guid>
    {
        public virtual Guid IdProductComponent { get; set; }        
        public virtual string? ConsumptionComponentLabel { get; set; }
        public virtual string? ConsumptionComponentCodeAndName { get; set; }
        public virtual string? ConsumptionComponentFormula { get; set; }
        public virtual bool IsValid { get; set; }

        protected ConsumptionProduct()
        {

        }

        public ConsumptionProduct(Guid id, Guid idProductComponent, 
            string? consumptionComponentLabel = null,  
            string? consumptionComponentCodeAndName = null,  
            string? consumptionComponentFormula = null, 
            bool isValid = false)
        {
            Id = id;
            IdProductComponent = idProductComponent;
            ConsumptionComponentCodeAndName = consumptionComponentCodeAndName;
            ConsumptionComponentLabel = consumptionComponentLabel;
            ConsumptionComponentFormula = consumptionComponentFormula;
            IsValid = isValid;
        }
    }
}