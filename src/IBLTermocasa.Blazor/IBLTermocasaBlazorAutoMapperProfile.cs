using AutoMapper;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Components;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Interactions;
using IBLTermocasa.Materials;
using IBLTermocasa.Organizations;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Subproducts;
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

        CreateMap<ProductDto, ProductUpdateDto>().Ignore(x => x.ComponentIds);

        CreateMap<SubproductDto, SubproductUpdateDto>();

        CreateMap<IndustryDto, IndustryUpdateDto>();
        CreateMap<ContactDto, ContactUpdateDto>();

        CreateMap<ContactDto, ContactCreateDto>();
        CreateMap<OrganizationDto, OrganizationUpdateDto>();
        CreateMap<OrganizationDto, OrganizationCreateDto>();

        CreateMap<InteractionDto, InteractionUpdateDto>();

        CreateMap<QuestionTemplateDto, QuestionTemplateUpdateDto>();

        CreateMap<RequestForQuotationDto, RequestForQuotationUpdateDto>();

        CreateMap<ProductDto, ProductUpdateDto>().Ignore(x => x.ComponentIds).Ignore(x => x.QuestionTemplateIds);

        CreateMap<CatalogDto, CatalogUpdateDto>().Ignore(x => x.ProductIds);
    }
}