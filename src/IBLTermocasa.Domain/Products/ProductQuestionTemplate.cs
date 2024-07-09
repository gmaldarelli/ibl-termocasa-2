using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplate : Entity<Guid>
    {
        
        [NotNull]
        public virtual Guid ParentId { get;  set; }
        [NotNull]
        public virtual Guid QuestionTemplateId { get; set; }
        [NotNull]
        public virtual int Order { get; set; }
        [NotNull]
        public virtual string Code { get; set; }
        [NotNull]
        public virtual string Name { get; set; }
        public virtual bool Mandatory { get; set; }
        
        private ProductQuestionTemplate()
        {

        }

        public ProductQuestionTemplate(Guid id, Guid parentId, Guid questionTemplateId, int order, string code, string name, bool mandatory)
        {
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Check.NotNull(questionTemplateId, nameof(questionTemplateId));
            Check.NotNull(parentId, nameof(parentId));
            Check.NotNull(id, nameof(id));
            Id = id;
            ParentId = parentId;
            QuestionTemplateId = questionTemplateId;
            Order = order;
            Code = code;
            Name = name;
            Mandatory = mandatory;
        }   

        public override object[] GetKeys()
        {
            return new object[]
                {
                    Id
                };
        }
    }
}