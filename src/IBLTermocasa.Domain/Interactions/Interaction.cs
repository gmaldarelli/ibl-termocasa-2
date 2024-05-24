using IBLTermocasa.Types;
using Volo.Abp.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Interactions
{
    public class Interaction : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual InteractionType InteractionType { get; set; }

        public virtual DateTime InteractionDate { get; set; }

        [CanBeNull]
        public virtual string? Content { get; set; }

        [CanBeNull]
        public virtual string? ReferenceObject { get; set; }

        [CanBeNull]
        public virtual string? WriterNotes { get; set; }
        public Guid WriterUserId { get; set; }
        public Guid? NotificationOrganizationUnitId { get; set; }
        public Guid? IdentityUserId { get; set; }

        protected Interaction()
        {

        }

        public Interaction(Guid id, Guid writerUserId, Guid? notificationOrganizationUnitId, Guid? identityUserId, InteractionType interactionType, DateTime interactionDate, string? content = null, string? referenceObject = null, string? writerNotes = null)
        {

            Id = id;
            Check.Length(content, nameof(content), InteractionConsts.ContentMaxLength, 0);
            InteractionType = interactionType;
            InteractionDate = interactionDate;
            Content = content;
            ReferenceObject = referenceObject;
            WriterNotes = writerNotes;
            WriterUserId = writerUserId;
            NotificationOrganizationUnitId = notificationOrganizationUnitId;
            IdentityUserId = identityUserId;
        }

    }
}