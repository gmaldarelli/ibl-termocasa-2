using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class GetProfessionalProfilesInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public double? StandardPrice { get; set; }
        public double? StandardPriceMin { get; set; }
        public double? StandardPriceMax { get; set; }

        public GetProfessionalProfilesInput()
        {

        }
    }
}