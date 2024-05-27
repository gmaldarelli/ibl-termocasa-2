using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplate : Entity
    {

        public Guid ProductId { get; protected set; }

        public Guid QuestionTemplateId { get; protected set; }

        private ProductQuestionTemplate()
        {

        }

        public ProductQuestionTemplate(Guid productId, Guid questionTemplateId)
        {
            ProductId = productId;
            QuestionTemplateId = questionTemplateId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    ProductId,
                    QuestionTemplateId
                };
        }
    }
}