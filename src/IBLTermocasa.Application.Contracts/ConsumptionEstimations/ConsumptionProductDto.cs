using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionProductDto : Entity<Guid>
    {
        public virtual Guid IdProductComponent { get; set; }
        public virtual string? ConsumptionComponentFormula { get; set; }
        public virtual bool IsValid { get; set; }

        protected ConsumptionProductDto()
        {

        }

        public ConsumptionProductDto(Guid id, Guid idProductComponent, string consumptionComponentFormula, bool isValid)
        {
            Id = id;
            IdProductComponent = idProductComponent;
            ConsumptionComponentFormula = consumptionComponentFormula;
            IsValid = isValid;
        }
    }
}