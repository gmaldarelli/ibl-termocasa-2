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

        var industryPermission = myGroup.AddPermission(IBLTermocasaPermissions.Industries.Default, L("Permission:Industries"));
        industryPermission.AddChild(IBLTermocasaPermissions.Industries.Create, L("Permission:Create"));
        industryPermission.AddChild(IBLTermocasaPermissions.Industries.Edit, L("Permission:Edit"));
        industryPermission.AddChild(IBLTermocasaPermissions.Industries.Delete, L("Permission:Delete"));

        var contactPermission = myGroup.AddPermission(IBLTermocasaPermissions.Contacts.Default, L("Permission:Contacts"));
        contactPermission.AddChild(IBLTermocasaPermissions.Contacts.Create, L("Permission:Create"));
        contactPermission.AddChild(IBLTermocasaPermissions.Contacts.Edit, L("Permission:Edit"));
        contactPermission.AddChild(IBLTermocasaPermissions.Contacts.Delete, L("Permission:Delete"));

        var organizationPermission = myGroup.AddPermission(IBLTermocasaPermissions.Organizations.Default, L("Permission:Organizations"));
        organizationPermission.AddChild(IBLTermocasaPermissions.Organizations.Create, L("Permission:Create"));
        organizationPermission.AddChild(IBLTermocasaPermissions.Organizations.Edit, L("Permission:Edit"));
        organizationPermission.AddChild(IBLTermocasaPermissions.Organizations.Delete, L("Permission:Delete"));

        var interactionPermission = myGroup.AddPermission(IBLTermocasaPermissions.Interactions.Default, L("Permission:Interactions"));
        interactionPermission.AddChild(IBLTermocasaPermissions.Interactions.Create, L("Permission:Create"));
        interactionPermission.AddChild(IBLTermocasaPermissions.Interactions.Edit, L("Permission:Edit"));
        interactionPermission.AddChild(IBLTermocasaPermissions.Interactions.Delete, L("Permission:Delete"));

        var questionTemplatePermission = myGroup.AddPermission(IBLTermocasaPermissions.QuestionTemplates.Default, L("Permission:QuestionTemplates"));
        questionTemplatePermission.AddChild(IBLTermocasaPermissions.QuestionTemplates.Create, L("Permission:Create"));
        questionTemplatePermission.AddChild(IBLTermocasaPermissions.QuestionTemplates.Edit, L("Permission:Edit"));
        questionTemplatePermission.AddChild(IBLTermocasaPermissions.QuestionTemplates.Delete, L("Permission:Delete"));

        var requestForQuotationPermission = myGroup.AddPermission(IBLTermocasaPermissions.RequestForQuotations.Default, L("Permission:RequestForQuotations"));
        requestForQuotationPermission.AddChild(IBLTermocasaPermissions.RequestForQuotations.Create, L("Permission:Create"));
        requestForQuotationPermission.AddChild(IBLTermocasaPermissions.RequestForQuotations.Edit, L("Permission:Edit"));
        requestForQuotationPermission.AddChild(IBLTermocasaPermissions.RequestForQuotations.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IBLTermocasaResource>(name);
    }
}