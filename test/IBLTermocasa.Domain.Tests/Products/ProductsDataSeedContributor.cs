using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Products;

namespace IBLTermocasa.Products
{
    public class ProductsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ProductsDataSeedContributor(IProductRepository productRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _productRepository = productRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"),
                code: "f89035412abd49ca901c6",
                name: "d149a7618e564b6cb0d2b86e2c43057e481f450333ed49f08121f6b04fb7ad5cf1f471d087",
                description: "eb9f92542c83476b99",
                isAssembled: true,
                isInternal: true
            ));

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("dc25b2b3-d2f4-4d83-87c3-41a58d8f3806"),
                code: "3be38c6dc52c4c229",
                name: "a07486fb37284c8896b272257ea15e1088424",
                description: "b5f09bc8a30c4e2caa7c820a8959885e3332b",
                isAssembled: true,
                isInternal: true
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}