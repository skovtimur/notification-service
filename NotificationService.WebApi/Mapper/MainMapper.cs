using AutoMapper;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Extensions;
using NotificationService.Domain.Models;

namespace NotificationService.WebApi.Mapper;

public class MainMapper : Profile
{
    public MainMapper()
    {
        CreateMap<TaskEntity, TaskDto>()
            .ForMember(dest => dest.RecipientsDtos, opt => opt.MapFrom(x => x.Recipients.ToDtos()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(x => x.Priority.ToString()))
            .ForMember(dest => dest.JsonContent, opt => opt.MapFrom(x => x.Content.JsonContent));

        CreateMap<TaskEntity, TaskParticleDto>()
            .ForMember(dest => dest.RecipientsDtos, opt => opt.MapFrom(x => x.Recipients.ToDtos()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(x => x.Priority.ToString()));

        CreateMap<RecipientEntity, RecipientDto>()
            .ForMember(d => d.SendingStatus, opt => opt.MapFrom(x => x.SendingStatus.ToString()));
    }
}