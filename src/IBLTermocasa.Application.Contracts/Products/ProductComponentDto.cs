using System;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Components;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products;

public class ProductComponentDto
{
    [Required]
    public int Order { get; set; }
    [Required]
    public string Code { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
    public bool Mandatory { get; set; }
    public Guid ComponentId { get;  set; }       
    public string? ConsumptionCalculation { get; set; }
}