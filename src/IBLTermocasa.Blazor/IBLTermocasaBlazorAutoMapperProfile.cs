using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.BillOfMaterials;
using AutoMapper;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Common;
using IBLTermocasa.Components;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Interactions;
using IBLTermocasa.Materials;
using IBLTermocasa.Organizations;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using Volo.Abp.AutoMapper;

namespace IBLTermocasa.Blazor;

public class IBLTermocasaBlazorAutoMapperProfile : Profile
{
    public IBLTermocasaBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<MaterialDto, MaterialUpdateDto>();

        CreateMap<ComponentDto, ComponentUpdateDto>();
        CreateMap<ComponentUpdateDto, ComponentDto>();
        CreateMap<ComponentDto, ComponentCreateDto>();
        CreateMap<ComponentCreateDto, ComponentDto>();
        CreateMap<ComponentItemDto, ComponentItem>();
        CreateMap<ComponentItem, ComponentItemDto>();

        CreateMap<ProductDto, ProductUpdateDto>();

        CreateMap<IndustryDto, IndustryUpdateDto>();
        CreateMap<ContactDto, ContactUpdateDto>();

        CreateMap<ContactDto, ContactCreateDto>();
        CreateMap<OrganizationDto, OrganizationUpdateDto>();
        CreateMap<OrganizationDto, OrganizationCreateDto>();

        CreateMap<InteractionDto, InteractionUpdateDto>();

        CreateMap<QuestionTemplateDto, QuestionTemplateUpdateDto>();

        CreateMap<RequestForQuotationDto, RequestForQuotationUpdateDto>();
        CreateMap<RequestForQuotationDto, RequestForQuotationCreateDto>();
        CreateMap<RequestForQuotationCreateDto, RequestForQuotation>();
        CreateMap<RequestForQuotation, RequestForQuotationDto>();
        CreateMap<RequestForQuotationItem, RequestForQuotationItemDto>();
        CreateMap<RequestForQuotationItemDto, RequestForQuotationItem>();

        CreateMap<ProductDto, ProductUpdateDto>();
        CreateMap<ProductDto, ProductCreateDto>();

        CreateMap<CatalogDto, CatalogUpdateDto>().Ignore(x => x.ProductIds);
        CreateMap<BillOfMaterial, BillOfMaterialDto>();
        CreateMap<BillOfMaterialDto, BillOfMaterial>();
        CreateMap<BillOfMaterialDto, BillOfMaterialCreateDto>();
        CreateMap<BillOfMaterialDto, BillOfMaterialUpdateDto>();
        CreateMap<BillOfMaterialCreateDto, BillOfMaterialDto>();
        CreateMap<BillOfMaterialUpdateDto, BillOfMaterialDto>();

        CreateMap<AddressDto, Address>();
        CreateMap<Address, AddressDto>();
        CreateMap<PhoneInfoDto, PhoneInfo>();
        CreateMap<PhoneInfo, PhoneInfoDto>();
        CreateMap<PhoneItemDto, PhoneItem>();
        CreateMap<PhoneItem, PhoneItemDto>();
        CreateMap<SocialInfoDto, SocialInfo>();
        CreateMap<SocialInfo, SocialInfoDto>();
        CreateMap<SocialItemDto, SocialItem>();
        CreateMap<SocialItem, SocialItemDto>();
        CreateMap<MailInfoDto, MailInfo>();
        CreateMap<MailInfo, MailInfoDto>();
        CreateMap<MailItemDto, MailItem>();
        CreateMap<MailItem, MailItemDto>();
        CreateMap<OrganizationPropertyDto, OrganizationProperty>();
        CreateMap<OrganizationProperty, OrganizationPropertyDto>();
        CreateMap<ContactPropertyDto, ContactProperty>();
        CreateMap<ContactProperty, ContactPropertyDto>();
        CreateMap<AgentPropertyDto, AgentProperty>();
        CreateMap<AgentProperty, AgentPropertyDto>();
        CreateMap<RequestForQuotationPropertyDto, RequestForQuotationProperty>();
        CreateMap<RequestForQuotationProperty, RequestForQuotationPropertyDto>();
        CreateMap<ProductItemDto, ProductItem>();
        CreateMap<ProductItem, ProductItemDto>();
        CreateMap<AnswerDto, Answer>();
        CreateMap<Answer, AnswerDto>();
        CreateMap<BomItem, BomItemDto>();
        CreateMap<BomItemDto, BomItem>();;
        CreateMap<BowItem, BowItemDto>();
        CreateMap<BowItemDto, BowItem>();
        CreateMap<BomComponent, BomComponentDto>();
        CreateMap<BomComponentDto, BomComponent>();
        CreateMap<BomItem, BomItem>();
        CreateMap<BomItemDto, BomItemDto>();
        CreateMap<BomProductItem, BomProductItemDto>();
        CreateMap<BomProductItemDto, BomProductItem>();

        CreateMap<ProfessionalProfile, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfileCreateDto, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfileUpdateDto, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfile>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfileCreateDto>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfileUpdateDto>();

        CreateMap<ConsumptionEstimation, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimationCreateDto, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimationUpdateDto, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimation>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimationCreateDto>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimationUpdateDto>();
    }
}