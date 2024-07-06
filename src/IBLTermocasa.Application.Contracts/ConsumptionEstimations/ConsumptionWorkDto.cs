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
    public class ConsumptionWorkDto : Entity<Guid>
    {
        public virtual Guid IdProfessionalProfile { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual string? ConsumptionWorkFormula { get; set; }
        public virtual int WorkTime { get; set; }

        protected ConsumptionWorkDto()
        {

        }

        public ConsumptionWorkDto(Guid id, Guid idProfessionalProfile, string name, double price, string consumptionWorkFormula, int workTime)
        {
            Id = id;
            IdProfessionalProfile = idProfessionalProfile;
            Name = name;
            Price = price;
            ConsumptionWorkFormula = consumptionWorkFormula;
            WorkTime = workTime;
        }
    }
}