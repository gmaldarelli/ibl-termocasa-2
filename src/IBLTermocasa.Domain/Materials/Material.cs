using IBLTermocasa.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Materials
{
    public abstract class MaterialBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual MeasureUnit MeasureUnit { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual decimal Lifo { get; set; }

        public virtual decimal StandardPrice { get; set; }

        public virtual decimal AveragePrice { get; set; }

        public virtual decimal LastPrice { get; set; }

        public virtual decimal AveragePriceSecond { get; set; }

        protected MaterialBase()
        {

        }

        public MaterialBase(Guid id, string code, string name, MeasureUnit measureUnit, decimal quantity, decimal lifo, decimal standardPrice, decimal averagePrice, decimal lastPrice, decimal averagePriceSecond)
        {

            Id = id;
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Code = code;
            Name = name;
            MeasureUnit = measureUnit;
            Quantity = quantity;
            Lifo = lifo;
            StandardPrice = standardPrice;
            AveragePrice = averagePrice;
            LastPrice = lastPrice;
            AveragePriceSecond = averagePriceSecond;
        }

    }
}