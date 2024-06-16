using System;
using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.BillOfMaterials;

public class BomComponentDto : EntityDto<Guid>
{
    public Guid ComponentId { get; set; }
    public string ComponentName { get; set; } = null!;
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public MeasureUnit MeasureUnit { get; set; } 
    public decimal Price { get; set; } = 0;
}