using AutoMapper;
using CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;

namespace CarDDD.ApplicationServices.Mappers;

public class CarMappingProfile : Profile
{
    public CarMappingProfile()
    {
        CreateMap<Car, CarInfo>()
            .ForMember(d => d.Id,          o => o.MapFrom(s => s.EntityId))
            .ForMember(d => d.Brand,       o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Color,       o => o.MapFrom(s => s.Color))
            .ForMember(d => d.Price,       o => o.MapFrom(s => s.Price))
            .ForMember(d => d.Mileage,     o => o.MapFrom(s => s.Mileage))
            .ForMember(d => d.Condition,   o => o.MapFrom(s => s.Condition))
            .ForMember(d => d.IsAvailable, o => o.MapFrom(s => s.IsAvailable))
            .ForMember(d => d.ManagerId,   o => o.MapFrom(s => s.ManagerId.Value))

            .ForMember(d => d.PhotoId, 
                o => o.MapFrom(s => s.Photo.Attached() 
                    ? (Guid?)s.Photo.Id 
                    : null))

            .ForMember(d => d.PhotoUrl, 
                o => o.Ignore());
    }
}