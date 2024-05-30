using System;

namespace IBLTermocasa.Common;

public class QuestionTemplateModel
{
    public Guid QuestionId { get; set; }
    public string AnswerValue { get; set; }
    public Guid ProductId { get; set; }
    
    public QuestionTemplateModel(Guid questionId, string answerValue, Guid productId)
    {
        QuestionId = questionId;
        AnswerValue = answerValue;
        ProductId = productId;
    }
    
    public QuestionTemplateModel()
    {
    }
}