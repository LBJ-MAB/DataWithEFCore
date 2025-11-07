using Application.Dtos;
using AutoMapper;
using Domain;

namespace Application.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<TaskItemDto, TaskItem>();
        CreateMap<TaskItem, TaskItemDto>();
    }
}