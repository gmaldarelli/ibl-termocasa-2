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
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual string? ConsumptionWorkFormula { get; set; }
        public virtual int WorkTime { get; set; }

        public ConsumptionWorkDto()
        {

        }

        public ConsumptionWorkDto(Guid id, Guid idProfessionalProfile, string code, string name, double price, string consumptionWorkFormula, int workTime)
        {
            Id = id;
            IdProfessionalProfile = idProfessionalProfile;
            Code = code;
            Name = name;
            Price = price;
            WorkTime = workTime;
            ConsumptionWorkFormula = consumptionWorkFormula;
        }
    }
}