using IBLTermocasa.Types;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Organizations
{
    public class OrganizationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? SocialInfo { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Tags { get; set; }
        public string? Notes { get; set; }
        public string? ImageId { get; set; }
        public Guid IndustryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}