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
    public class ConsumptionEstimation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string? ConsumptionProduct { get; set; }

        [CanBeNull]
        public virtual string? ConsumptionWork { get; set; }

        protected ConsumptionEstimation()
        {

        }

        public ConsumptionEstimation(Guid id, string? consumptionProduct = null, string? consumptionWork = null)
        {

            Id = id;
            ConsumptionProduct = consumptionProduct;
            ConsumptionWork = consumptionWork;
        }

    }
}