using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.QuestionTemplates;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplatesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IQuestionTemplateRepository _questionTemplateRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public QuestionTemplatesDataSeedContributor(IQuestionTemplateRepository questionTemplateRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _questionTemplateRepository = questionTemplateRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _questionTemplateRepository.InsertAsync(new QuestionTemplate
            (
                id: Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"),
                code: "2f8174834",
                questionText: "f4bb275d1a18403aa9fe29cd5da9b90b0e55bed932b44d5ea87a9b16fd07104609b8a44e6ad4416b8d67698",
                answerType: default,
                choiceValue: "09275452846c4fec8a9ad1421eb6a482984564c1844a42cf86f1392ead6ca65"
            ));

            await _questionTemplateRepository.InsertAsync(new QuestionTemplate
            (
                id: Guid.Parse("2bac9d7f-dec5-4b0e-b2b1-7712e713cc57"),
                code: "ef9d397450b649dd9b8aaf6a090ac4e474db9313612f4f3fb2c4d9564774aa68fdd71672258d4428abf5a25",
                questionText: "2c64db89196e4b588573fc8f783bef5043f03b2c3bc4493aa180a60300d57dee9eb6f3adbccc408",
                answerType: default,
                choiceValue: "2fe4a70929724f2a8e00d23fa661c53775e3979386704a5ebf7e4ee86a16f6ff16a"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}