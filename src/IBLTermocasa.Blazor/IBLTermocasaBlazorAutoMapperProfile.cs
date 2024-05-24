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

        CreateMap<OrganizationDto, OrganizationUpdateDto>();

        CreateMap<InteractionDto, InteractionUpdateDto>();

        CreateMap<QuestionTemplateDto, QuestionTemplateUpdateDto>();
    }
}