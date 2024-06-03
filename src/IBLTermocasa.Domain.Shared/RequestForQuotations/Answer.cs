using System;

namespace IBLTermocasa.RequestForQuotations;

public class Answer
{
    public Guid ProductId { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerValue { get; set; } = null!;

    public Answer(Guid productId, Guid questionId, string answerValue)
    {
        ProductId = productId;
        QuestionId = questionId;
        AnswerValue = answerValue;
    }

    public Answer()
    {
    }
}