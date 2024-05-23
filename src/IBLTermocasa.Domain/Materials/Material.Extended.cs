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
    public class Material : MaterialBase
    {
        //<suite-custom-code-autogenerated>
        protected Material()
        {

        }

        public Material(Guid id, string code, string name, MeasureUnit measureUnit, decimal quantity, decimal lifo, decimal standardPrice, decimal averagePrice, decimal lastPrice, decimal averagePriceSecond)
            : base(id, code, name, measureUnit, quantity, lifo, standardPrice, averagePrice, lastPrice, averagePriceSecond)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
    }
}