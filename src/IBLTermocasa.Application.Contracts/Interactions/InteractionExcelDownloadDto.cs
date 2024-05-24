using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Interactions
{
    public class InteractionExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public InteractionType? InteractionType { get; set; }
        public DateTime? InteractionDateMin { get; set; }
        public DateTime? InteractionDateMax { get; set; }
        public string? Content { get; set; }
        public string? ReferenceObject { get; set; }
        public string? WriterNotes { get; set; }
        public Guid? WriterUserId { get; set; }
        public Guid? IdentityUserId { get; set; }

        public InteractionExcelDownloadDto()
        {

        }
    }
}