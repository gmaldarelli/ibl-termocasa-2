using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
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

        public virtual async Task<ConsumptionEstimation> CreateAsync(
        string? consumptionProduct = null, string? consumptionWork = null)
        {

            var consumptionEstimation = new ConsumptionEstimation(
             GuidGenerator.Create(),
             consumptionProduct, consumptionWork
             );

            return await _consumptionEstimationRepository.InsertAsync(consumptionEstimation);
        }

        public virtual async Task<ConsumptionEstimation> UpdateAsync(
            Guid id,
            string? consumptionProduct = null, string? consumptionWork = null, [CanBeNull] string? concurrencyStamp = null
        )
        {

            var consumptionEstimation = await _consumptionEstimationRepository.GetAsync(id);

            consumptionEstimation.ConsumptionProduct = consumptionProduct;
            consumptionEstimation.ConsumptionWork = consumptionWork;

            consumptionEstimation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _consumptionEstimationRepository.UpdateAsync(consumptionEstimation);
        }

    }
}