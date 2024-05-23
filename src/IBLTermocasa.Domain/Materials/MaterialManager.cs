using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Materials
{
    public abstract class MaterialManagerBase : DomainService
    {
        protected IMaterialRepository _materialRepository;

        public MaterialManagerBase(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        public virtual async Task<Material> CreateAsync(
        string code, string name, MeasureUnit measureUnit, decimal quantity, decimal lifo, decimal standardPrice, decimal averagePrice, decimal lastPrice, decimal averagePriceSecond)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(measureUnit, nameof(measureUnit));

            var material = new Material(
             GuidGenerator.Create(),
             code, name, measureUnit, quantity, lifo, standardPrice, averagePrice, lastPrice, averagePriceSecond
             );

            return await _materialRepository.InsertAsync(material);
        }

        public virtual async Task<Material> UpdateAsync(
            Guid id,
            string code, string name, MeasureUnit measureUnit, decimal quantity, decimal lifo, decimal standardPrice, decimal averagePrice, decimal lastPrice, decimal averagePriceSecond, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(measureUnit, nameof(measureUnit));

            var material = await _materialRepository.GetAsync(id);

            material.Code = code;
            material.Name = name;
            material.MeasureUnit = measureUnit;
            material.Quantity = quantity;
            material.Lifo = lifo;
            material.StandardPrice = standardPrice;
            material.AveragePrice = averagePrice;
            material.LastPrice = lastPrice;
            material.AveragePriceSecond = averagePriceSecond;

            material.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _materialRepository.UpdateAsync(material);
        }

    }
}