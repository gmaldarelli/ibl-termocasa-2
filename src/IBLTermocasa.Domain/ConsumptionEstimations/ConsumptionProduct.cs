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
        public virtual string? ConsumptionComponentFormula { get; set; }
        public virtual bool IsValid { get; set; }

        protected ConsumptionProduct()
        {

        }

        public ConsumptionProduct(Guid id, Guid idProductComponent, string consumptionComponentFormula, bool isValid)
        {
            Id = id;
            IdProductComponent = idProductComponent;
            ConsumptionComponentFormula = consumptionComponentFormula;
            IsValid = isValid;
        }
    }
}