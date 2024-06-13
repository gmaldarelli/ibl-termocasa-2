using System;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.BillOFMaterials;

public class BOMComponentDto : EntityDto<Guid>
{
    public Guid ComponentId { get; set; }
    public string ComponentName { get; set; } = null!;
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; } = 0;
}