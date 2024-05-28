using System;

namespace IBLTermocasa.RequestForQuotations;

public class Answer
{
    public Guid QuestionId { get; set; }
    public string AnswerValue { get; set; } = null!;

    public Answer(Guid questionId, string answerValue)
    {
        QuestionId = questionId;
        AnswerValue = answerValue;
    }

    public Answer()
    {
    }
}