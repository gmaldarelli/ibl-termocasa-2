using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplate : Entity
    {

        public Guid ProductId { get;  set; }

        public Guid QuestionTemplateId { get; set; }

        [NotNull]
        public virtual int Order { get; set; }
        
        [NotNull]
        public virtual string Name { get; set; }
        public virtual bool Mandatory { get; set; }
        
        private ProductQuestionTemplate()
        {

        }

        public ProductQuestionTemplate(Guid productId, Guid questionTemplateId, int order, bool mandatory)
        {
            ProductId = productId;
            QuestionTemplateId = questionTemplateId;
            Order = order;
            Mandatory = mandatory;
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