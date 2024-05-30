using System;
using System.Collections.Generic;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;

namespace IBLTermocasa.RequestForQuotations;

public class RFQProductAndQuestionDto
{
    public ProductDto Product { get; set; }
    public List<QuestionTemplateDto> QuestionTemplates { get; set; }

    public RFQProductAndQuestionDto(ProductDto product, List<QuestionTemplateDto> questionTemplates)
    {
        Product = product;
        QuestionTemplates = questionTemplates;
    }

    public RFQProductAndQuestionDto()
    {
    }
}