using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionProductDto : Entity<Guid>
    {
        public virtual Guid IdProductComponent { get; set; }
        public virtual string? ConsumptionComponentLabel { get; set; }
        public virtual string? ConsumptionComponentCodeAndName { get; set; }
        public virtual string? ConsumptionComponentFormula { get; set; }
        public virtual bool IsValid { get; set; }

        protected ConsumptionProductDto()
        {

        }

        public ConsumptionProductDto(Guid id, Guid idProductComponent, 
            string? consumptionComponentLabel = null,  
            string? consumptionComponentCodeAndName = null,  
            string? consumptionComponentFormula = null, 
            bool isValid = false)
        {
            Id = id;
            IdProductComponent = idProductComponent;
            ConsumptionComponentFormula = consumptionComponentFormula;
            ConsumptionComponentLabel = consumptionComponentLabel;
            ConsumptionComponentCodeAndName = consumptionComponentCodeAndName;
            IsValid = isValid;
        }
    }
}