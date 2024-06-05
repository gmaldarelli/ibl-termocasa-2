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


        CreateMap<Product, ProductDto>()/*
            .Ignore(x => x.ProductComponents).Ignore(x => x.ProductQuestionTemplates).Ignore(x => x.SubProducts)
            .AfterMap((src, dest) => 
            {
                dest.ProductComponents = src.ProductComponents.Select(pc => new ProductComponentDto 
                {
                    Id = Guid.NewGuid(),
                    Order = pc.Order,
                    Mandatory = pc.Mandatory,
                    Component = new ComponentDto
                    {
                        Id = pc.ComponentId
                    }
                }).ToList();
                dest.ProductQuestionTemplates = src.ProductQuestionTemplates.Select(pqt => new ProductQuestionTemplateDto
                {
                    Id = Guid.NewGuid(),
                    QuestionTemplate = new QuestionTemplateDto
                    {
                        Id = pqt.QuestionTemplateId
                    }
                }).ToList();
            })*/;
            /*
            .Ignore(x => x.ProductComponents).Ignore(x => x.ProductQuestionTemplates)
            .Ignore(x => x.ProductComponents)
            .AfterMap((src, dest) => 
            {
                dest.ProductComponents = src.ProductComponents.Select(c => new ComponentDto 
                {
                    Id = c.ComponentId
                }).ToList();
                dest.ProductQuestionTemplates = src.ProductQuestionTemplates.Select(qt => new QuestionTemplateDto
                {
                    // Mappatura delle proprietà di QuestionTemplateDto
                    // Ad esempio, se QuestionTemplateDto ha una proprietà "Id", potrebbe essere mappata così:
                    Id = qt.QuestionTemplateId
                }).ToList();
            });
            */
            CreateMap<ProductDto,Product >()
                .Ignore(x => x.SubProducts).Ignore(x => x.ProductComponents).Ignore(x => x.ProductQuestionTemplates);
                /*.AfterMap((src, dest) =>
                {
                    src.ProductComponents.Select(pc => new ProductComponent(
                        pc.Id,
                        pc.Component.Id,
                        pc.Order,
                        pc.Mandatory
                    )).ToList().ForEach(x => dest.ProductComponents.Add(x));
                    src.ProductQuestionTemplates.Select(pqt => new ProductQuestionTemplate(
                        pqt.Id,
                        pqt.QuestionTemplate.Id,
                        pqt.Order,
                        pqt.Mandatory
                    )).ToList().ForEach(x => dest.ProductQuestionTemplates.Add(x));
                    src.SubProducts.Select(sp => new SubProduct(
                        sp.ParentId,
                        sp.Products.Select(x => x.Id).ToList(),
                        sp.Order,
                        sp.Name,
                        sp.IsSingleProduct,
                        sp.Mandatory
                    )).ToList().ForEach(x => dest.SubProducts.Add(x));
                });*/
        CreateMap<Product, ProductExcelDto>();
        CreateMap<Component, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<SubProduct, SubProductDto>();
            /*.Ignore(x => x.Products)
            .AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.Products = src.ProductIds.Select(p => new ProductDto
                {
                    Id = p
                }).ToList();
            });*/
            CreateMap<SubProductDto, SubProduct>();
            /*.Ignore(x => x.ProductIds)
            .AfterMap((src, dest) =>
            {
                src.Products.Select(p => p.Id).ToList().ForEach(x => dest.ProductIds.Add(x));
            });*/
        CreateMap<ProductComponent, ProductComponentDto>()
            /*.Ignore(x => x.Component)
            .AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.Component = new ComponentDto
                {
                    Id = src.ComponentId
                };
            })*/;
        CreateMap<ProductComponentDto, ProductComponent>()/*.Ignore(x => x.ComponentId)
            .AfterMap((src, dest) =>
            {
                dest.ComponentId = src.Component.Id;
            })*/;
        
        CreateMap<ProductQuestionTemplate, ProductQuestionTemplateDto>()/*.Ignore(x => x.QuestionTemplate)
            .AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.QuestionTemplate = new QuestionTemplateDto
                {
                    Id = src.QuestionTemplateId
                };
            })*/;
        CreateMap<ProductQuestionTemplateDto, ProductQuestionTemplate>()/*.Ignore(x => x.QuestionTemplateId)
            .AfterMap((src, dest) =>
            {
                dest.QuestionTemplateId = src.QuestionTemplate.Id;
            })*/;
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
        CreateMap<OrganizationUpdateDto, Organization>();
        CreateMap<OrganizationCreateDto, Organization>();
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
            .ForMember(dest => dest.OrganizationProperty, opt => opt.MapFrom(src => src.OrganizationProperty));
        CreateMap<RequestForQuotationItem, RequestForQuotationItemDto>();
        CreateMap<RequestForQuotationItemDto, RequestForQuotationItem>();
        CreateMap<RequestForQuotationUpdateDto, RequestForQuotation>();
        CreateMap<RequestForQuotation, RequestForQuotationUpdateDto>();
        CreateMap<RequestForQuotation, RequestForQuotationCreateDto>();
        CreateMap<RequestForQuotation, RequestForQuotationDto>();
        CreateMap<RequestForQuotation, RequestForQuotationExcelDto>();
        CreateMap<RequestForQuotationWithNavigationProperties, RequestForQuotationWithNavigationPropertiesDto>();
        CreateMap<Contact, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<Organization, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        
        CreateMap<Catalog, CatalogDto>();
        CreateMap<Catalog, CatalogExcelDto>();
        CreateMap<CatalogWithNavigationProperties, CatalogWithNavigationPropertiesDto>();
            /*
            .Ignore(x => x.ProductComponents).Ignore(x => x.ProductQuestionTemplates)
            .Ignore(x => x.ProductComponents)
            .AfterMap((src, dest) => 
            {
                dest.ProductComponents = src.ProductComponents.Select(c => new ComponentDto 
                {
                    Id = c.ComponentId
                }).ToList();
                dest.ProductQuestionTemplates = src.ProductQuestionTemplates.Select(qt => new QuestionTemplateDto
                {
                    // Mappatura delle proprietà di QuestionTemplateDto
                    // Ad esempio, se QuestionTemplateDto ha una proprietà "Id", potrebbe essere mappata così:
                    Id = qt.QuestionTemplateId
                }).ToList();
            });*/
        CreateMap<QuestionTemplate, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.QuestionText));

        CreateMap<Component, ComponentDto>().Ignore(x => x.ComponentItems);
    }
}