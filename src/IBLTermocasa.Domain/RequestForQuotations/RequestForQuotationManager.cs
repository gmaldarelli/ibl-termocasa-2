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

        public virtual async Task<RequestForQuotation> CreateAsync(
        Guid? agentId, Guid? contactId, Guid? organizationId, string quoteNumber, decimal discount, Status status, string? workSite = null, string? city = null, string? organizationProperty = null, string? contactProperty = null, string? phoneInfo = null, string? mailInfo = null, string? description = null)
        {
            Check.NotNullOrWhiteSpace(quoteNumber, nameof(quoteNumber));
            Check.NotNull(status, nameof(status));

            var requestForQuotation = new RequestForQuotation(
             GuidGenerator.Create(),
             agentId, contactId, organizationId, quoteNumber, discount, status, workSite, city, organizationProperty, contactProperty, phoneInfo, mailInfo, description
             );

            return await _requestForQuotationRepository.InsertAsync(requestForQuotation);
        }

        public virtual async Task<RequestForQuotation> UpdateAsync(
            Guid id,
            Guid? agentId, Guid? contactId, Guid? organizationId, string quoteNumber, decimal discount, Status status, string? workSite = null, string? city = null, string? organizationProperty = null, string? contactProperty = null, string? phoneInfo = null, string? mailInfo = null, string? description = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(quoteNumber, nameof(quoteNumber));
            Check.NotNull(status, nameof(status));

            var requestForQuotation = await _requestForQuotationRepository.GetAsync(id);

            requestForQuotation.AgentId = agentId;
            requestForQuotation.ContactId = contactId;
            requestForQuotation.OrganizationId = organizationId;
            requestForQuotation.QuoteNumber = quoteNumber;
            requestForQuotation.Discount = discount;
            requestForQuotation.Status = status;
            requestForQuotation.WorkSite = workSite;
            requestForQuotation.City = city;
            requestForQuotation.OrganizationProperty = organizationProperty;
            requestForQuotation.ContactProperty = contactProperty;
            requestForQuotation.PhoneInfo = phoneInfo;
            requestForQuotation.MailInfo = mailInfo;
            requestForQuotation.Description = description;

            requestForQuotation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _requestForQuotationRepository.UpdateAsync(requestForQuotation);
        }

    }
}