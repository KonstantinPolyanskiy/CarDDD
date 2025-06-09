using System.Collections.Immutable;
using CarDDD.DomainServices.DomainAggregates.CarAggregate;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Models.AnswerObjects.ServiceResponses;

public record CarInfo
{
    public Guid Id { get; set; }
    
    public string? Brand { get; set; } 
    public string? Color { get; set; } 
    public decimal? Price { get; set; }
    public int? Mileage { get; set; }
    
        
    public Condition? Condition { get; set; }
    public bool? IsAvailable { get; set; }
    
    public Guid? ManagerId { get; set; }
    public Guid? PhotoId { get; set; }
    public string? PhotoUrl { get; set; }
}



public record UserInfo
{
    public Guid? Id { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    public string? Login  { get; set; }

    public ImmutableArray<Role>? Roles { get; set; } = [];
}

public record PhotoInfo
{
    public Guid? Id { get; set; }
    
    public string? Extension { get; set; }
    public ImmutableArray<byte>? Data { get; set; }
}