using IBLTermocasa.Catalogs;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Identity;
using IBLTermocasa.Interactions;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Subproducts;
using IBLTermocasa.Products;
using IBLTermocasa.Components;
using System;
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

        CreateMap<Component, ComponentDto>();
        CreateMap<Component, ComponentExcelDto>();
        CreateMap<ComponentCreateDto, Component>();
        CreateMap<ComponentUpdateDto, Component>();
        CreateMap<ComponentItemDto, ComponentItem>();
        CreateMap<ComponentItem, ComponentItemDto>().Ignore(x => x.MaterialCode).Ignore(x => x.MaterialName);
        CreateMap<ComponentWithNavigationProperties, ComponentItemDto>();
        CreateMap<Material, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        

        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductExcelDto>();
        CreateMap<ProductWithNavigationProperties, ProductWithNavigationPropertiesDto>();
        CreateMap<Component, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Subproduct, SubproductDto>();
        CreateMap<SubproductWithNavigationProperties, SubproductWithNavigationPropertiesDto>();
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
        CreateMap<Contact, ContactDto>();

        CreateMap<Organization, OrganizationDto>();
        CreateMap<Organization, OrganizationExcelDto>();
        CreateMap<OrganizationWithNavigationProperties, OrganizationWithNavigationPropertiesDto>();
        CreateMap<Industry, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Description));
        CreateMap<OrganizationPropertyDto, OrganizationProperty>();
        CreateMap<OrganizationProperty, OrganizationPropertyDto>();
        CreateMap<OrganizationUpdateDto, Organization>();
        CreateMap<OrganizationCreateDto, Organization>();
        CreateMap<Organization, OrganizationDto>();
        CreateMap<Organization, OrganizationExcelDto>();
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
        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.NormalizedUserName));
        CreateMap<OrganizationUnit, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName));
        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.NormalizedUserName));

        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserName));
        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserName));

        CreateMap<QuestionTemplate, QuestionTemplateDto>();
        CreateMap<QuestionTemplate, QuestionTemplateExcelDto>();

        CreateMap<RequestForQuotation, RequestForQuotationDto>();
        CreateMap<RequestForQuotation, RequestForQuotationExcelDto>();
        CreateMap<RequestForQuotationWithNavigationProperties, RequestForQuotationWithNavigationPropertiesDto>();
        CreateMap<Contact, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<Organization, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Product, ProductDto>().Ignore(x => x.Subproducts);
        CreateMap<QuestionTemplate, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.QuestionText));

        CreateMap<Component, ComponentDto>().Ignore(x => x.ComponentItems);

        CreateMap<Catalog, CatalogDto>();
        CreateMap<Catalog, CatalogExcelDto>();
        CreateMap<CatalogWithNavigationProperties, CatalogWithNavigationPropertiesDto>();
    }
}