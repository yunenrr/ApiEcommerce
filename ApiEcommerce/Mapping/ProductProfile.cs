using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        /*
        src: Representa el objeto de origen (source), es decir, el objeto desde el cual se están obteniendo los datos. Por ejemplo, si tienes una entidad Product, src sería una instancia de esa clase.
        dest: Representa el objeto de destino (destination), es decir, el objeto al que se están asignando los datos mapeados. Por ejemplo, podría ser un DTO (Data Transfer Object) llamado ProductDto.
        opt: Es una abreviatura de "options" y representa las opciones de configuración para el mapeo. Permite especificar detalles adicionales sobre cómo se debe realizar el mapeo.
        */
        CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)) // Mapea el nombre de la categoría desde la entidad Category
        .ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }
}
