using IBLTermocasa.Quotations;
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Catalogs;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Identity;
using IBLTermocasa.Interactions;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Products;
using IBLTermocasa.Components;
using System;
using System.Linq;
using IBLTermocasa.Shared;
using Volo.Abp.AutoMapper;
using IBLTermocasa.Materials;
using AutoMapper;
using IBLTermocasa.Common;

namespace IBLTermocasa;

public class IBLTermocasaApplicationAutoMapperProfile : Profile
{
    public IBLTermocasaApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Material, MaterialDto>();
        CreateMap<Material, MaterialExcelDto>();

        CreateMap<Component, ComponentExcelDto>();
        CreateMap<ComponentCreateDto, Component>();
        CreateMap<ComponentUpdateDto, Component>();
        CreateMap<ComponentItemDto, ComponentItem>();
        CreateMap<ComponentItem, ComponentItemDto>().Ignore(x => x.MaterialCode).Ignore(x => x.MaterialName);
        CreateMap<ComponentWithNavigationProperties, ComponentItemDto>();
        CreateMap<Material, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>()
            .Ignore(x => x.SubProducts).Ignore(x => x.ProductComponents).Ignore(x => x.ProductQuestionTemplates);
        CreateMap<Product, ProductExcelDto>();
        CreateMap<Component, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<SubProduct, SubProductDto>();
        CreateMap<SubProductDto, SubProduct>();
        CreateMap<ProductComponent, ProductComponentDto>();
        CreateMap<ProductComponentDto, ProductComponent>();

        CreateMap<ProductQuestionTemplate, ProductQuestionTemplateDto>();
        CreateMap<ProductQuestionTemplateDto, ProductQuestionTemplate>();
        CreateMap<Product, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Industry, IndustryDto>();

        CreateMap<Contact, ContactDto>();
        CreateMap<Contact, ContactExcelDto>();
        CreateMap<ContactPropertyDto, ContactProperty>();
        CreateMap<ContactProperty, ContactPropertyDto>();
        CreateMap<ContactCreateDto, Contact>();
        CreateMap<ContactUpdateDto, Contact>();
        CreateMap<Contact, ContactCreateDto>();
        CreateMap<Contact, ContactUpdateDto>();

        CreateMap<Organization, OrganizationDto>();
        CreateMap<Organization, OrganizationExcelDto>();
        CreateMap<OrganizationWithNavigationProperties, OrganizationWithNavigationPropertiesDto>();
        CreateMap<Industry, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Description));
        CreateMap<OrganizationPropertyDto, OrganizationProperty>();
        CreateMap<OrganizationProperty, OrganizationPropertyDto>();
        CreateMap<AgentPropertyDto, AgentProperty>();
        CreateMap<AgentProperty, AgentPropertyDto>();
        CreateMap<OrganizationUpdateDto, Organization>();
        CreateMap<OrganizationCreateDto, Organization>();
        CreateMap<RequestForQuotationPropertyDto, RequestForQuotationProperty>();
        CreateMap<RequestForQuotationProperty, RequestForQuotationPropertyDto>();
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

        CreateMap<Interaction, InteractionDto>();
        CreateMap<Interaction, InteractionExcelDto>();
        CreateMap<InteractionWithNavigationProperties, InteractionWithNavigationPropertiesDto>();
        CreateMap<OrganizationUnit, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName));
        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserName));

        CreateMap<QuestionTemplate, QuestionTemplateDto>();
        CreateMap<QuestionTemplate, QuestionTemplateExcelDto>();

        CreateMap<RequestForQuotationDto, RequestForQuotationUpdateDto>();
        CreateMap<RequestForQuotationDto, RequestForQuotationCreateDto>();
        CreateMap<RequestForQuotationCreateDto, RequestForQuotation>()
            .ForMember(dest => dest.ContactProperty, opt => opt.MapFrom(src => src.ContactProperty))
            .ForMember(dest => dest.OrganizationProperty, opt => opt.MapFrom(src => src.OrganizationProperty))
            .ForMember(dest => dest.AgentProperty, opt => opt.MapFrom(src => src.AgentProperty));
        CreateMap<RequestForQuotationItem, RequestForQuotationItemDto>();
        CreateMap<RequestForQuotationItemDto, RequestForQuotationItem>();
        CreateMap<RequestForQuotationUpdateDto, RequestForQuotation>();
        CreateMap<RequestForQuotation, RequestForQuotationUpdateDto>();
        CreateMap<RequestForQuotation, RequestForQuotationCreateDto>();
        CreateMap<RequestForQuotation, RequestForQuotationDto>();
        CreateMap<RequestForQuotation, RequestForQuotationExcelDto>();
        CreateMap<RequestForQuotationWithNavigationProperties, RequestForQuotationWithNavigationPropertiesDto>();
        CreateMap<ProductItemDto, ProductItem>();
        CreateMap<ProductItem, ProductItemDto>();
        CreateMap<AnswerDto, Answer>();
        CreateMap<Answer, AnswerDto>();
        CreateMap<Contact, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name + " " + src.Surname));
        CreateMap<Organization, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<RequestForQuotation, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.QuoteNumber));

        CreateMap<Catalog, CatalogDto>();
        CreateMap<CatalogProduct, CatalogProductDto>();
        CreateMap<CatalogProductDto, CatalogProduct>();
        CreateMap<Catalog, CatalogExcelDto>();
        CreateMap<CatalogWithNavigationProperties, CatalogWithNavigationPropertiesDto>();
        CreateMap<QuestionTemplate, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.QuestionText));

        CreateMap<Component, ComponentDto>().Ignore(x => x.ComponentItems);

        CreateMap<BillOfMaterial, BillOfMaterialDto>();
        CreateMap<BillOfMaterialDto, BillOfMaterial>();
        CreateMap<BillOfMaterialDto, BillOfMaterialCreateDto>();
        CreateMap<BillOfMaterialCreateDto, BillOfMaterialDto>();
        CreateMap<BillOfMaterialUpdateDto, BillOfMaterial>();
        CreateMap<BillOfMaterialDto, BillOfMaterialUpdateDto>();
        CreateMap<BomItem, BomItemDto>();
        CreateMap<BomItemDto, BomItem>();
        CreateMap<BowItem, BowItemDto>();
        CreateMap<BowItemDto, BowItem>();
        CreateMap<BomComponent, BomComponentDto>();
        CreateMap<BomItem, BomItem>();
        CreateMap<BomItemDto, BomItemDto>();
        CreateMap<BomComponentDto, BomComponent>();
        CreateMap<BomProductItem, BomProductItemDto>();
        CreateMap<BomProductItemDto, BomProductItem>();

        CreateMap<ProfessionalProfile, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfile, ProfessionalProfileExcelDto>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfile>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfileCreateDto>();
        CreateMap<ProfessionalProfileDto, ProfessionalProfileUpdateDto>();
        CreateMap<ProfessionalProfileCreateDto, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfileCreateDto, ProfessionalProfile>();
        CreateMap<ProfessionalProfileUpdateDto, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfileUpdateDto, ProfessionalProfile>();
        CreateMap<ProfessionalProfile, ProfessionalProfileDto>();
        CreateMap<ProfessionalProfile, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<ConsumptionEstimation, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimation, ConsumptionEstimationExcelDto>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimation>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimationCreateDto>();
        CreateMap<ConsumptionEstimationDto, ConsumptionEstimationUpdateDto>();
        CreateMap<ConsumptionEstimationCreateDto, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimationCreateDto, ConsumptionEstimation>();
        CreateMap<ConsumptionEstimationUpdateDto, ConsumptionEstimationDto>();
        CreateMap<ConsumptionEstimationUpdateDto, ConsumptionEstimation>();
        CreateMap<ConsumptionEstimation, ConsumptionEstimationDto>();
        CreateMap<ConsumptionProduct, ConsumptionProductDto>();
        CreateMap<ConsumptionProductDto, ConsumptionProduct>();
        CreateMap<ConsumptionWorkDto, ConsumptionWork>();
        CreateMap<ConsumptionWork, ConsumptionWorkDto>();
        
        CreateMap<QuotationDto, QuotationUpdateDto>();
        CreateMap<QuotationDto, QuotationCreateDto>();
        CreateMap<QuotationCreateDto, Quotation>();
        CreateMap<QuotationItem, QuotationItemDto>();
        CreateMap<QuotationItemDto, QuotationItem>();
        CreateMap<QuotationUpdateDto, Quotation>();
        CreateMap<Quotation, QuotationUpdateDto>();
        CreateMap<Quotation, QuotationCreateDto>();
        CreateMap<Quotation, QuotationDto>();
        CreateMap<Quotation, QuotationExcelDto>();
    }
}