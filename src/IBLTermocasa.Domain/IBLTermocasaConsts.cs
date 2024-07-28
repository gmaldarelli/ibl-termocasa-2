using System.Collections.Generic;
using Volo.Abp.Identity;

namespace IBLTermocasa;

public static class IBLTermocasaConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
    public static double? Markup { get; set; } = 30;
    public static List<double> MarkUps { get; set; } = [30, 15, 50];
}
