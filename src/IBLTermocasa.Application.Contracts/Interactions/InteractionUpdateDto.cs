using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Interactions
{
    public class InteractionUpdateDto : IHasConcurrencyStamp
    {
        public InteractionType InteractionType { get; set; }
        public DateTime InteractionDate { get; set; }
        [StringLength(InteractionConsts.ContentMaxLength)]
        public string? Content { get; set; }
        public string? ReferenceObject { get; set; }
        public string? WriterNotes { get; set; }
        public Guid WriterUserId { get; set; }
        public Guid? NotificationOrganizationUnitId { get; set; }
        public Guid? IdentityUserId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}