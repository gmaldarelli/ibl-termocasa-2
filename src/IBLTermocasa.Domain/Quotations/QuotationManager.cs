using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace IBLTermocasa.Quotations
{
    public class QuotationManager : DomainService
    {
        protected IQuotationRepository _quotationRepository;

        public QuotationManager(IQuotationRepository quotationRepository)
        {
            _quotationRepository = quotationRepository;
        }

        public virtual async Task<Quotation> CreateAsync(Quotation quotation)
        {
            Check.NotNull(quotation, nameof(quotation));
            return await _quotationRepository.InsertAsync(quotation);
        }

        public virtual async Task<Quotation> UpdateAsync(Guid id, Quotation quotation)
        {
            Check.NotNull(quotation, nameof(quotation));
            var existingQuotation = await _quotationRepository.GetAsync(id);
            Quotation.FillPropertiesForUpdate(quotation, existingQuotation);
            return await _quotationRepository.UpdateAsync(existingQuotation);
        }
    }
}