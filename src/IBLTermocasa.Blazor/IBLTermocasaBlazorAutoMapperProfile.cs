using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Interactions;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Subproducts;
using IBLTermocasa.Products;
using IBLTermocasa.ComponentItems;
using IBLTermocasa.Components;
using Volo.Abp.AutoMapper;
using IBLTermocasa.Materials;
using AutoMapper;
using IBLTermocasa.Catalogs;

namespace IBLTermocasa.Blazor;

public class IBLTermocasaBlazorAutoMapperProfile : Profile
{
    public IBLTermocasaBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<MaterialDto, MaterialUpdateDto>();

        CreateMap<ComponentDto, ComponentUpdateDto>();

        CreateMap<ComponentItemDto, ComponentItemUpdateDto>();

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
        CreateMap<RequestForQuotationDto, RequestForQuotationCreateDto>();
        CreateMap<RequestForQuotationCreateDto, RequestForQuotation>();
        CreateMap<RequestForQuotationUpdateDto, RequestForQuotation>();
        CreateMap<RequestForQuotation, RequestForQuotationUpdateDto>();
        CreateMap<RequestForQuotation, RequestForQuotationCreateDto>();
    
        CreateMap<ProductDto, ProductUpdateDto>().Ignore(x => x.ComponentIds).Ignore(x => x.QuestionTemplateIds);

        CreateMap<CatalogDto, CatalogUpdateDto>().Ignore(x => x.ProductIds);
    }
}