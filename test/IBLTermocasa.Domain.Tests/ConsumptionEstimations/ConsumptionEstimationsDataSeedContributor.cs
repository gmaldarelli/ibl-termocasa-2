using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.ConsumptionEstimations;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimationsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IConsumptionEstimationRepository _consumptionEstimationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ConsumptionEstimationsDataSeedContributor(IConsumptionEstimationRepository consumptionEstimationRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _consumptionEstimationRepository = consumptionEstimationRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _consumptionEstimationRepository.InsertAsync(new ConsumptionEstimation
            (
                id: Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"),
                idProduct: Guid.Parse("ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51"),
                consumptionProduct: new List<ConsumptionProduct>(),
                consumptionWork: new List<ConsumptionWork>()
            ));

            await _consumptionEstimationRepository.InsertAsync(new ConsumptionEstimation
            (
                id: Guid.Parse("4d3f667a-a8ac-416e-8290-a4cac7c1863b"),
                idProduct: Guid.Parse("ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51"),
                consumptionProduct: new List<ConsumptionProduct>(),
                consumptionWork: new List<ConsumptionWork>()
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}