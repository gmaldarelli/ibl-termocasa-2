using IBLTermocasa.Types;
using System;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateExcelDto
    {
        public string Code { get; set; } = null!;
        public string QuestionText { get; set; } = null!;
        public AnswerType AnswerType { get; set; }
    }
}