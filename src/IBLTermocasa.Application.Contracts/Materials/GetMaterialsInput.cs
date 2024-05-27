using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Materials
{
    public class GetMaterialsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }

        public GetMaterialsInput()
        {

        }
    }
}