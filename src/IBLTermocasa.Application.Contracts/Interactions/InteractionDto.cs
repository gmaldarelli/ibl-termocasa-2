using IBLTermocasa.Types;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Interactions
{
    public class InteractionDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public InteractionType InteractionType { get; set; }
        public DateTime InteractionDate { get; set; }
        public string? Content { get; set; }
        public string? ReferenceObject { get; set; }
        public string? WriterNotes { get; set; }
        public Guid WriterUserId { get; set; }
        public Guid? NotificationOrganizationUnitId { get; set; }
        public Guid? IdentityUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}