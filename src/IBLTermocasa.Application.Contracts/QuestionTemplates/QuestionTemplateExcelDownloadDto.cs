using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? QuestionText { get; set; }
        public AnswerType? AnswerType { get; set; }
        public string? ChoiceValue { get; set; }

        public QuestionTemplateExcelDownloadDto()
        {

        }
    }
}