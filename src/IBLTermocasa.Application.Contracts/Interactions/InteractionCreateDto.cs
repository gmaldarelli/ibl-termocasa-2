using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Interactions
{
    public class InteractionCreateDto
    {
        public InteractionType InteractionType { get; set; } = ((InteractionType[])Enum.GetValues(typeof(InteractionType)))[0];
        public DateTime InteractionDate { get; set; }
        [StringLength(InteractionConsts.ContentMaxLength)]
        public string? Content { get; set; }
        public string? ReferenceObject { get; set; }
        public string? WriterNotes { get; set; }
        public Guid WriterUserId { get; set; }
        public Guid? NotificationOrganizationUnitId { get; set; }
        public Guid? IdentityUserId { get; set; }
    }
}