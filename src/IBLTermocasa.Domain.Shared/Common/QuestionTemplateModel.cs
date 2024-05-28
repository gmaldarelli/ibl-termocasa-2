using System;

namespace IBLTermocasa.Common;

public class QuestionTemplateModel
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    
    public QuestionTemplateModel(Guid id, string value)
    {
        Id = id;
        Value = value;
    }
    
    public QuestionTemplateModel()
    {
    }
}