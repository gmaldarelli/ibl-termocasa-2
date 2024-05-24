using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Industries
{
    public class Industry : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        protected Industry()
        {

        }

        public Industry(Guid id, string code, string? description = null)
        {

            Id = id;
            Check.NotNull(code, nameof(code));
            Check.Length(code, nameof(code), IndustryConsts.CodeMaxLength, 0);
            Check.Length(description, nameof(description), IndustryConsts.DescriptionMaxLength, 0);
            Code = code;
            Description = description;
        }

    }
}