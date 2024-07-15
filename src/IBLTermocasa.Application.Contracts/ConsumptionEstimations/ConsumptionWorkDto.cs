using System;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionWorkDto : EntityDto<Guid>
    {
        public Guid IdProfessionalProfile { get; set; }
        public Guid ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string? ConsumptionWorkFormula { get; set; }
        public int WorkTime { get; set; }

        public ConsumptionWorkDto()
        {
            
        }
        public ConsumptionWorkDto(Guid id)
        {
            Id = id;
        }
        public ConsumptionWorkDto(Guid id, Guid idProfessionalProfile, Guid productId, string code, string name, double price, string consumptionWorkFormula, int workTime)
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