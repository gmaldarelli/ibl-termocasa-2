using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Materials;

namespace IBLTermocasa.Controllers.Materials
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Material")]
    [Route("api/app/materials")]

    public class MaterialController : MaterialControllerBase, IMaterialsAppService
    {
        public MaterialController(IMaterialsAppService materialsAppService) : base(materialsAppService)
        {
        }
    }
}