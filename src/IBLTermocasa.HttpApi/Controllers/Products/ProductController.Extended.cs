using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Products;

namespace IBLTermocasa.Controllers.Products
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Product")]
    [Route("api/app/products")]

    public class ProductController : ProductControllerBase, IProductsAppService
    {
        public ProductController(IProductsAppService productsAppService) : base(productsAppService)
        {
        }
    }
}