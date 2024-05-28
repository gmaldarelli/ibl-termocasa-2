using IBLTermocasa.RequestForQuotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationManager : DomainService
    {
        protected IRequestForQuotationRepository _requestForQuotationRepository;

        public RequestForQuotationManager(IRequestForQuotationRepository requestForQuotationRepository)
        {
            _requestForQuotationRepository = requestForQuotationRepository;
        }

        public virtual async Task<RequestForQuotation> CreateAsync(RequestForQuotation requestForQuotation)
        {
            Check.NotNull(requestForQuotation, nameof(requestForQuotation));
            return await _requestForQuotationRepository.InsertAsync(requestForQuotation);
        }

        public virtual async Task<RequestForQuotation> UpdateAsync(Guid id, RequestForQuotation requestForQuotation)
        {
            Check.NotNull(requestForQuotation, nameof(requestForQuotation));
            var existingRequestForQuotation = await _requestForQuotationRepository.GetAsync(id);
            RequestForQuotation.FillPropertiesForUpdate(requestForQuotation, existingRequestForQuotation);
            return await _requestForQuotationRepository.UpdateAsync(existingRequestForQuotation);
        }
    }
}