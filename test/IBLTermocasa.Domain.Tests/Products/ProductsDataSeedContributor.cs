using System;
using System.Collections.Generic;
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
                id: Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"),
                code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                name: "f687499a4f344537b9852cbfb81d806cf42c2406d21443aa867ba5e8",
                description: "0f648430cf6f466d9ff18a67fda002e06a22abd9d9e7408a922aadaddb07fae9fcaf7916b8f74aaf801c4b1ff594ab",
                isAssembled: true,
                isInternal: true,
                subProducts: new List<SubProduct>()
                {
                    new SubProduct(
                        Guid.Parse("f1b1b3b4-1b3b-4b1b-b3b4-1b3b4b1b3b4b"), 
                        productIds: new List<Guid> { Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c") },
                        order: 1,
                        code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                        name: "Name1",
                        mandatory: true,
                        isSingleProduct: true),
                    new SubProduct(
                        id: Guid.Parse("f1b1b3b4-1b3b-4b1b-b3b4-1b3b4b1b3b4b"),
                        productIds: new List<Guid> { Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c") },
                        order: 2,
                        code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                        name: "Name2",
                        mandatory: true,
                        isSingleProduct: true)
                        
                }
            ));

            await _productRepository.InsertAsync(new Product
            (
                id: Guid.Parse("58980b43-af35-4248-aa0e-fecdefd22bad"),
                code: "53b6bf2d62964a9bbaebe7a61fd8a9e5074a648ec0a64a4cbbfc14262ecd9641dfa072a",
                name: "67b09f03c87e46e8b9106089a41e4a4ce4062ee641b34af5b4",
                description: "64338505adae42d0876b44aa265b8232e",
                isAssembled: true,
                isInternal: true,
                subProducts: new List<SubProduct>()
                {
                    new SubProduct(
                        Guid.Parse("f1b1b3b4-1b3b-4b1b-b3b4-1b3b4b1b3b4b"), 
                        productIds: new List<Guid> { Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c") },
                        order: 1,
                        code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                        name: "Name1",
                        mandatory: true,
                        isSingleProduct: true),
                    new SubProduct(
                        id: Guid.Parse("f1b1b3b4-1b3b-4b1b-b3b4-1b3b4b1b3b4b"),
                        productIds: new List<Guid> { Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c") },
                        order: 2,
                        code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                        name: "Name2",
                        mandatory: true,
                        isSingleProduct: true)
                        
                }
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}