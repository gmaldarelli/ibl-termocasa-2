using IBLTermocasa.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace IBLTermocasa.Permissions;

public class IBLTermocasaPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(IBLTermocasaPermissions.GroupName);

        myGroup.AddPermission(IBLTermocasaPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(IBLTermocasaPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(IBLTermocasaPermissions.MyPermission1, L("Permission:MyPermission1"));

        var materialPermission = myGroup.AddPermission(IBLTermocasaPermissions.Materials.Default, L("Permission:Materials"));
        materialPermission.AddChild(IBLTermocasaPermissions.Materials.Create, L("Permission:Create"));
        materialPermission.AddChild(IBLTermocasaPermissions.Materials.Edit, L("Permission:Edit"));
        materialPermission.AddChild(IBLTermocasaPermissions.Materials.Delete, L("Permission:Delete"));

        var componentPermission = myGroup.AddPermission(IBLTermocasaPermissions.Components.Default, L("Permission:Components"));
        componentPermission.AddChild(IBLTermocasaPermissions.Components.Create, L("Permission:Create"));
        componentPermission.AddChild(IBLTermocasaPermissions.Components.Edit, L("Permission:Edit"));
        componentPermission.AddChild(IBLTermocasaPermissions.Components.Delete, L("Permission:Delete"));

        var componentItemPermission = myGroup.AddPermission(IBLTermocasaPermissions.ComponentItems.Default, L("Permission:ComponentItems"));
        componentItemPermission.AddChild(IBLTermocasaPermissions.ComponentItems.Create, L("Permission:Create"));
        componentItemPermission.AddChild(IBLTermocasaPermissions.ComponentItems.Edit, L("Permission:Edit"));
        componentItemPermission.AddChild(IBLTermocasaPermissions.ComponentItems.Delete, L("Permission:Delete"));

        var productPermission = myGroup.AddPermission(IBLTermocasaPermissions.Products.Default, L("Permission:Products"));
        productPermission.AddChild(IBLTermocasaPermissions.Products.Create, L("Permission:Create"));
        productPermission.AddChild(IBLTermocasaPermissions.Products.Edit, L("Permission:Edit"));
        productPermission.AddChild(IBLTermocasaPermissions.Products.Delete, L("Permission:Delete"));

        var subproductPermission = myGroup.AddPermission(IBLTermocasaPermissions.Subproducts.Default, L("Permission:Subproducts"));
        subproductPermission.AddChild(IBLTermocasaPermissions.Subproducts.Create, L("Permission:Create"));
        subproductPermission.AddChild(IBLTermocasaPermissions.Subproducts.Edit, L("Permission:Edit"));
        subproductPermission.AddChild(IBLTermocasaPermissions.Subproducts.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IBLTermocasaResource>(name);
    }
}