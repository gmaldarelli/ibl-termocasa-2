using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationManager : DomainService
    {
        protected IConsumptionEstimationRepository _consumptionEstimationRepository;

        public ConsumptionEstimationManager(IConsumptionEstimationRepository consumptionEstimationRepository)
        {
            _consumptionEstimationRepository = consumptionEstimationRepository;
        }

        public virtual async Task<ConsumptionEstimation> CreateAsync(ConsumptionEstimation consumptionEstimation)
        {
            Check.NotNull(consumptionEstimation, nameof(consumptionEstimation));
            return await _consumptionEstimationRepository.InsertAsync(consumptionEstimation);
        }

        public virtual async Task<ConsumptionEstimation> UpdateAsync(Guid id, ConsumptionEstimation consumptionEstimation)
        {
            Check.NotNull(consumptionEstimation, nameof(consumptionEstimation));
            var existingConsumptionEstimation = await _consumptionEstimationRepository.GetAsync(id);
            ConsumptionEstimation.FillPropertiesForUpdate(consumptionEstimation, existingConsumptionEstimation);
            return await _consumptionEstimationRepository.UpdateAsync(existingConsumptionEstimation);
        }

    }
}