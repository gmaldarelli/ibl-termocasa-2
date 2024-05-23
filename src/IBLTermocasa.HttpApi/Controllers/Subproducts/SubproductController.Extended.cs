using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Subproducts;

namespace IBLTermocasa.Controllers.Subproducts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Subproduct")]
    [Route("api/app/subproducts")]

    public class SubproductController : SubproductControllerBase, ISubproductsAppService
    {
        public SubproductController(ISubproductsAppService subproductsAppService) : base(subproductsAppService)
        {
        }
    }
}