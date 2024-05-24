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

namespace IBLTermocasa.Organizations
{
    public class OrganizationManager : DomainService
    {
        protected IOrganizationRepository _organizationRepository;

        public OrganizationManager(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public virtual async Task<Organization> CreateAsync(
        Guid industryId, string code, string name, OrganizationType organizationType, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? billingAddress = null, string? shippingAddress = null, string? tags = null, string? notes = null, string? imageId = null)
        {
            Check.NotNull(industryId, nameof(industryId));
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(organizationType, nameof(organizationType));

            var organization = new Organization(
             GuidGenerator.Create(),
             industryId, code, name, organizationType, mailInfo, phoneInfo, socialInfo, billingAddress, shippingAddress, tags, notes, imageId
             );

            return await _organizationRepository.InsertAsync(organization);
        }

        public virtual async Task<Organization> UpdateAsync(
            Guid id,
            Guid industryId, string code, string name, OrganizationType organizationType, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? billingAddress = null, string? shippingAddress = null, string? tags = null, string? notes = null, string? imageId = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(industryId, nameof(industryId));
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(organizationType, nameof(organizationType));

            var organization = await _organizationRepository.GetAsync(id);

            organization.IndustryId = industryId;
            organization.Code = code;
            organization.Name = name;
            organization.OrganizationType = organizationType;
            organization.MailInfo = mailInfo;
            organization.PhoneInfo = phoneInfo;
            organization.SocialInfo = socialInfo;
            organization.BillingAddress = billingAddress;
            organization.ShippingAddress = shippingAddress;
            organization.Tags = tags;
            organization.Notes = notes;
            organization.ImageId = imageId;

            organization.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _organizationRepository.UpdateAsync(organization);
        }

    }
}