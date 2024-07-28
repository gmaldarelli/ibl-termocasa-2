using System;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionWork : Entity<Guid>
    {
        public virtual Guid IdProfessionalProfile { get; set; }
        public virtual CostType? CostType { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual double Price { get; set; }
        public virtual int WorkTime { get; set; }
        public virtual string? ConsumptionWorkFormula { get; set; }

        protected ConsumptionWork()
        {

        }

        public ConsumptionWork(Guid id, Guid idProfessionalProfile, Guid productId, string code, string name, double price, string consumptionWorkFormula, int workTime)
        {
            Id = id;
            IdProfessionalProfile = idProfessionalProfile;
            ProductId = productId;
            Code = code;
            Name = name;
            Price = price;
            WorkTime = workTime;
            ConsumptionWorkFormula = consumptionWorkFormula;
        }
    }
}