using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Materials
{
    public abstract class GetMaterialsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }

        public GetMaterialsInputBase()
        {

        }
    }
}