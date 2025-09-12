using System;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap(); // Mapear entre Category y CategoryDto
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
    }
}
