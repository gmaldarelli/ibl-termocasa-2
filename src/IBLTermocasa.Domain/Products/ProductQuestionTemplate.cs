using System;
using JetBrains.Annotations;
using Volo.Abp;
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
        public virtual string Code { get; set; }
        [NotNull]
        public virtual string Name { get; set; }
        public virtual bool Mandatory { get; set; }
        
        public virtual string? ValidationFormula { get; set; }
        
        private ProductQuestionTemplate()
        {

        }

        public ProductQuestionTemplate(Guid productId, Guid questionTemplateId, int order, string code, string name, bool mandatory, string? validationFormula = null)
        {
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Check.NotNull(questionTemplateId, nameof(questionTemplateId));
            Check.NotNull(productId, nameof(productId));
            ProductId = productId;
            QuestionTemplateId = questionTemplateId;
            Order = order;
            Code = code;
            Name = name;
            Mandatory = mandatory;
            ValidationFormula = validationFormula;
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