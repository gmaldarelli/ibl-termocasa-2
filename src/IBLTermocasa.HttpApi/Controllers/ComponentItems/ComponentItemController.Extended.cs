using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.ComponentItems;

namespace IBLTermocasa.Controllers.ComponentItems
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ComponentItem")]
    [Route("api/app/component-items")]

    public class ComponentItemController : ComponentItemControllerBase, IComponentItemsAppService
    {
        public ComponentItemController(IComponentItemsAppService componentItemsAppService) : base(componentItemsAppService)
        {
        }
    }
}