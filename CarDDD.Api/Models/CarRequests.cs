using System.Text.Json.Serialization;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.Api.Models;

public class AddCarRequest
{
    public string Brand { get; set; } = string.Empty;
    
    public string Color { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int Mileage { get; set; }
    
    public IFormFile? Photo { get; set; }
}

public class UpdateCarRequest
{
    public string? Brand { get; set; }
    public string? Color { get; set; }
    public decimal? Price { get; set; }
    public int? Mileage { get; set; }

    public IFormFile? PhotoFile { get; set; }
    public bool OnlyIfNonePhoto { get; set; }

    public Guid? NewManagerId      { get; set; }
    public List<Role>? NewManagerRoles { get; set; }
}