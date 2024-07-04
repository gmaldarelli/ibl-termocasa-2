using System;
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
                consumptionProduct: "df77227afb4249b9b6af520810481f",
                consumptionWork: "5088cd37f1a241b08108f9d9fb5097ceccca5cb0280d4f39a2fb177f208c72587be8c553d48149cab1f38e"
            ));

            await _consumptionEstimationRepository.InsertAsync(new ConsumptionEstimation
            (
                id: Guid.Parse("4d3f667a-a8ac-416e-8290-a4cac7c1863b"),
                consumptionProduct: "da0e467a83cc4bd3aa0b75",
                consumptionWork: "1c68d608a39c4b48ac67c9665d3dbce9b293ad9740a8"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}