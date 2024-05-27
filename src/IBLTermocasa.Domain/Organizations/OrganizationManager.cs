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

        public virtual async Task<Organization> CreateAsync(Organization organization)
        {
            Check.NotNull(organization, nameof(organization));
            return await _organizationRepository.InsertAsync(organization);
        }

        public virtual async Task<Organization> UpdateAsync(Guid id, Organization organization)
        {
            Check.NotNull(organization, nameof(organization));
            var existingOrganization = await _organizationRepository.GetAsync(id);
            Organization.FillPropertiesForUpdate(organization, existingOrganization);
            return await _organizationRepository.UpdateAsync(existingOrganization);
        }

    }
}