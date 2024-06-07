using System;
using IBLTermocasa.Types;

namespace IBLTermocasa.Common;

public class QuestionTemplateModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public bool IsAssembled { get; set; }
    public Guid QuestionId { get; set; }
    public String QuestionText { get; set; }
    public AnswerType AnswerType { get; set; }
    public string AnswerValue { get; set; }

    public QuestionTemplateModel(Guid productId, string productName, bool isAssembled, Guid questionId, string questionText, AnswerType answerType, string answerValue)
    {
        ProductId = productId;
        ProductName = productName;
        IsAssembled = isAssembled;
        QuestionId = questionId;
        QuestionText = questionText;
        AnswerType = answerType;
        AnswerValue = answerValue;
    }
    
    public QuestionTemplateModel()
    {
    }
}