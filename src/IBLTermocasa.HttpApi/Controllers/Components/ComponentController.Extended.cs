using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Components;

namespace IBLTermocasa.Controllers.Components
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Component")]
    [Route("api/app/components")]

    public class ComponentController : ComponentControllerBase, IComponentsAppService
    {
        public ComponentController(IComponentsAppService componentsAppService) : base(componentsAppService)
        {
        }
    }
}