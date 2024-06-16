﻿using System;
using IBLTermocasa.Types;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BomComponent : Entity<Guid>
{
    public Guid ComponentId { get; set; }
    public string ComponentName { get; set; } = null!;
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; } = null!;
    public decimal MaterialPrice { get; set; }
    public decimal Quantity { get; set; }
    public MeasureUnit MeasureUnit { get; set; }
    public decimal Price { get; set; }

    public BomComponent(Guid id, Guid componentId, string componentName, Guid materialId, string materialName,
        decimal materialPrice,  MeasureUnit measureUnit, decimal quantity = 0, decimal price = 0)
    {
        Id = id;
        ComponentId = componentId;
        ComponentName = componentName;
        MaterialId = materialId;
        MaterialName = materialName;
        MaterialPrice = materialPrice;
        Quantity = quantity;
        MeasureUnit = measureUnit;
        Price = price;
    }
}