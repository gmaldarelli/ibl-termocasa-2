using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOFMaterials;

public class BOMComponent : Entity<Guid>
{
    public Guid ComponentId { get; set; }
    public string ComponentName { get; set; } = null!;
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; }
}