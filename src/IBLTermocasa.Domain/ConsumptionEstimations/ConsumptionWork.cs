using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionWork : Entity<Guid>
    {
        public virtual Guid IdProfessionalProfile { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual string? ConsumptionWorkFormula { get; set; }
        public virtual int WorkTime { get; set; }

        protected ConsumptionWork()
        {

        }

        public ConsumptionWork(Guid id, Guid idProfessionalProfile, string name, double price, string consumptionWorkFormula, int workTime)
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