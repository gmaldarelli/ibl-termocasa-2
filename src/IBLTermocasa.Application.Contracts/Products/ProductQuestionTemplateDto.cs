using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplateDto : Entity
    {

        public Guid ProductId { get; protected set; }

        public Guid QuestionTemplateId { get; protected set; }

        private ProductQuestionTemplateDto()
        {

        }

        public ProductQuestionTemplateDto(Guid productId, Guid questionTemplateId)
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