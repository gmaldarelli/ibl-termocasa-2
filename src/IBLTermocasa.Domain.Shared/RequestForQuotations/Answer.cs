using System;

namespace IBLTermocasa.RequestForQuotations;

public class Answer
{
    public Guid ProductItemId { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerValue { get; set; } = null!;

    public Answer(Guid productItemId, Guid questionId, string answerValue)
    {
        ProductItemId = productItemId;
        QuestionId = questionId;
        AnswerValue = answerValue;
    }

    public Answer()
    {
    }
}