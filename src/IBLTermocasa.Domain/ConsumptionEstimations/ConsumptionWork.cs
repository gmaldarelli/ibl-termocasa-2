using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionWork : Entity<Guid>
    {
        public virtual Guid IdProfessionalProfile { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual int WorkTime { get; set; }
        public virtual string? ConsumptionWorkFormula { get; set; }

        protected ConsumptionWork()
        {

        }

        public ConsumptionWork(Guid id, Guid idProfessionalProfile, string code, string name, double price, string consumptionWorkFormula, int workTime)
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