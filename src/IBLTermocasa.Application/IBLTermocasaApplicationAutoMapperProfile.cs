using IBLTermocasa.Subproducts;
using IBLTermocasa.Products;
using IBLTermocasa.ComponentItems;
using IBLTermocasa.Components;
using System;
using IBLTermocasa.Shared;
using Volo.Abp.AutoMapper;
using IBLTermocasa.Materials;
using AutoMapper;

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
        CreateMap<ComponentWithNavigationProperties, ComponentWithNavigationPropertiesDto>();
        CreateMap<Material, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<ComponentItem, ComponentItemDto>();
        CreateMap<ComponentItemWithNavigationProperties, ComponentItemWithNavigationPropertiesDto>();

        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductExcelDto>();
        CreateMap<ProductWithNavigationProperties, ProductWithNavigationPropertiesDto>();
        CreateMap<Component, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Subproduct, SubproductDto>();
        CreateMap<SubproductWithNavigationProperties, SubproductWithNavigationPropertiesDto>();
        CreateMap<Product, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}