using System;
using IBLTermocasa.Types;

namespace IBLTermocasa.RequestForQuotations;

public class Answer
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = null!;
    public AnswerType AnswerType { get; set; }
    public string AnswerValue { get; set; } = null!;

    public Answer(Guid questionId, string questionText, AnswerType answerType, string answerValue)
    {
        QuestionId = questionId;
        QuestionText = questionText;
        AnswerType = answerType;
        AnswerValue = answerValue;
    }

    public Answer()
    {
    }
}