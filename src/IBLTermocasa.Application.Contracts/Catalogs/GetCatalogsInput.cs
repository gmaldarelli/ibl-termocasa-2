using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Catalogs
{
    public class GetCatalogsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }
        public DateTime? FromMin { get; set; }
        public DateTime? FromMax { get; set; }
        public DateTime? ToMin { get; set; }
        public DateTime? ToMax { get; set; }
        public string? Description { get; set; }

        public GetCatalogsInput()
        {

        }
    }
}