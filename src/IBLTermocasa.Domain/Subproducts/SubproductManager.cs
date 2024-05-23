using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace IBLTermocasa.Subproducts
{
    public abstract class SubproductManagerBase : DomainService
    {
        protected ISubproductRepository _subproductRepository;

        public SubproductManagerBase(ISubproductRepository subproductRepository)
        {
            _subproductRepository = subproductRepository;
        }

        public virtual async Task<Subproduct> CreateAsync(
        Guid productId, Guid? singleProductId, int order, string name, bool isSingleProduct, bool mandatory)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var subproduct = new Subproduct(
             GuidGenerator.Create(),
             productId, singleProductId, order, name, isSingleProduct, mandatory
             );

            return await _subproductRepository.InsertAsync(subproduct);
        }

        public virtual async Task<Subproduct> UpdateAsync(
            Guid id,
            Guid productId, Guid? singleProductId, int order, string name, bool isSingleProduct, bool mandatory
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var subproduct = await _subproductRepository.GetAsync(id);

            subproduct.ProductId = productId;
            subproduct.SingleProductId = singleProductId;
            subproduct.Order = order;
            subproduct.Name = name;
            subproduct.IsSingleProduct = isSingleProduct;
            subproduct.Mandatory = mandatory;

            return await _subproductRepository.UpdateAsync(subproduct);
        }

    }
}