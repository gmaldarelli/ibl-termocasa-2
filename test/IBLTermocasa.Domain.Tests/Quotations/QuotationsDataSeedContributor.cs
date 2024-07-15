using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Quotations;
using IBLTermocasa.RequestForQuotations;

namespace IBLTermocasa.Quotations
{
    public class QuotationsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IQuotationRepository _quotationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public QuotationsDataSeedContributor(IQuotationRepository quotationRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _quotationRepository = quotationRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _quotationRepository.InsertAsync(new Quotation
            (
                id: Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"),
                idRFQ: Guid.Parse("e53078c4d0404571a05a2437af24dc8fc27853d56c1144f7b59db1d315128d321e"),
                idBOM: Guid.Parse("191f85bf475744319617019753a5b21c518a766e54d44b16a96c1063c37950e5"),
                code: "83dbc9bb41704e1e9c6514c599e24938971ad25f82a34e28",
                name: "708d2b1e0a9c4c69b0ea1ac76bb284856619176f52b0468781cae46b7f27a0dd9bf15f6e74544b2e9",
                sentDate: new DateTime(2000, 3, 23),
                quotationValidDate: new DateTime(2002, 6, 7),
                confirmedDate: new DateTime(2018, 11, 13),
                status: default,
                depositRequired: true,
                depositRequiredValue: 624446116,
                quotationItems: new List<QuotationItem>()
            ));

            await _quotationRepository.InsertAsync(new Quotation
            (
                id: Guid.Parse("c91c4ceb-10c8-459b-af22-be9e5b4ab2e9"),
                idRFQ: Guid.Parse("e53078c4d0404571a05a2437af24dc8fc27853d56c1144f7b59db1d315128d321e"),
                idBOM: Guid.Parse("191f85bf475744319617019753a5b21c518a766e54d44b16a96c1063c37950e5"),
                code: "34494dd14c864b1e9b6a02ee296b00952cb6a7a974",
                name: "54e3ef38c2874560825f89041bbc35727b7fc542e8d44e14a2b73888db39f806aa8b31780b524b37ab9",
                sentDate: new DateTime(2008, 6, 12),
                quotationValidDate: new DateTime(2012, 6, 6),
                confirmedDate: new DateTime(2005, 9, 12),
                status: default,
                depositRequired: true,
                depositRequiredValue: 1887778833,
                quotationItems: new List<QuotationItem>()
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}