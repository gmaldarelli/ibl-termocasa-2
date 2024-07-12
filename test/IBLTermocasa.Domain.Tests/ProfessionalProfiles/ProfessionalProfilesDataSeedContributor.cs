using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.ProfessionalProfiles;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfilesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IProfessionalProfileRepository _professionalProfileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProfessionalProfilesDataSeedContributor(IProfessionalProfileRepository professionalProfileRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _professionalProfileRepository = professionalProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _professionalProfileRepository.InsertAsync(new ProfessionalProfile
            (
                id: Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"),
                code: "dec40a5d478746f8bdc180237ec848fd5bbd1d5fe54245e0",
                name: "dec40a5d478746f8bdc180237ec848fd5bbd1d5fe54245e0",
                standardPrice: 1492575415
            ));

            await _professionalProfileRepository.InsertAsync(new ProfessionalProfile
            (
                id: Guid.Parse("b8b8926a-e8d6-4163-ab88-0865e1b588d7"),
                code: "87735376e0c44ff8b23d064089c041b4087320f0938a4a5187c7481d78",
                name: "87735376e0c44ff8b23d064089c041b4087320f0938a4a5187c7481d78",
                standardPrice: 1188708294
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}