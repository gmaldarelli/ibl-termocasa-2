using IBLTermocasa.Materials;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.Components;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Components
{
    public class ComponentsAppService : ComponentsAppServiceBase, IComponentsAppService
    {
        //<suite-custom-code-autogenerated>
        public ComponentsAppService(IComponentRepository componentRepository, ComponentManager componentManager, IDistributedCache<ComponentExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
            : base(componentRepository, componentManager, excelDownloadTokenCache)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
    }
}