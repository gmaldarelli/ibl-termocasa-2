using IBLTermocasa.Industries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Organizations;
using IBLTermocasa.Types;

namespace IBLTermocasa.Organizations
{
    public class OrganizationsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IndustriesDataSeedContributor _industriesDataSeedContributor;

        public OrganizationsDataSeedContributor(IOrganizationRepository organizationRepository, IUnitOfWorkManager unitOfWorkManager, IndustriesDataSeedContributor industriesDataSeedContributor)
        {
            _organizationRepository = organizationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _industriesDataSeedContributor = industriesDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _organizationRepository.InsertAsync(new Organization(
                id: Guid.NewGuid(), // Generate a new Guid for the id
                name: "Organization 1",
                industryId: Guid.NewGuid(), // or provide a Guid
                organizationType: OrganizationType.CUSTOMER, // or provide an OrganizationType
                shippingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                billingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                socialInfo: new SocialInfo
                {
                    // Fill in the properties for the SocialInfo
                },
                phoneInfo: new PhoneInfo
                {
                    // Fill in the properties for the PhoneInfo
                },
                mailInfo: new MailInfo
                {
                    // Fill in the properties for the MailInfo
                },
                tags: new List<string> { "tag1", "tag2" },
                imageId: null, // or provide a Guid
                notes: "This is a note for Organization 1"
            ));

            await _organizationRepository.InsertAsync(new Organization(
                id: Guid.NewGuid(), // Generate a new Guid for the id
                name: "Organization 2",
                industryId: Guid.NewGuid(), // or provide a Guid
                organizationType: OrganizationType.CUSTOMER, // or provide an OrganizationType
                shippingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                billingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                socialInfo: new SocialInfo
                {
                    // Fill in the properties for the SocialInfo
                },
                phoneInfo: new PhoneInfo
                {
                    // Fill in the properties for the PhoneInfo
                },
                mailInfo: new MailInfo
                {
                    // Fill in the properties for the MailInfo
                },
                tags: new List<string> { "tag3", "tag4" },
                imageId: null, // or provide a Guid
                notes: "This is a note for Organization 2"
            ));

            await _organizationRepository.InsertAsync(new Organization(
                id: Guid.NewGuid(), // Generate a new Guid for the id
                name: "Organization 3",
                industryId: Guid.NewGuid(), // or provide a Guid
                organizationType: OrganizationType.CUSTOMER, // or provide an OrganizationType
                shippingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                billingAddress: new Address
                {
                    // Fill in the properties for the Address
                },
                socialInfo: new SocialInfo
                {
                    // Fill in the properties for the SocialInfo
                },
                phoneInfo: new PhoneInfo
                {
                    // Fill in the properties for the PhoneInfo
                },
                mailInfo: new MailInfo
                {
                    // Fill in the properties for the MailInfo
                },
                tags: new List<string> { "tag5", "tag6" },
                imageId: null, // or provide a Guid
                notes: "This is a note for Organization 3"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}