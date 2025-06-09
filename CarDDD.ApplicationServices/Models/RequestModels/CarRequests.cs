using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.ApplicationServices.Models.RequestModels;

public record CreateCarRequest
{
    public string Brand { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal Price { get; init; } = decimal.Zero;
    public int Mileage { get; init; } = 0;
    
    public Guid EmployerId { get; init; } = Guid.Empty;
    public IReadOnlyList<Role> EmployerRoles { get; init; } = Array.Empty<Role>();
    
    public string PhotoExtension { get; init; } = string.Empty;
    public byte[]? PhotoData { get; init; } = null;
}

public record SellCarRequest
{
    public Guid CarId { get; init; } = Guid.Empty;
    public Guid CustomerId { get; init; } = Guid.Empty;
}

public record EditCarRequest
{
    public Guid CarId { get; init; }
    public string? Brand { get; init; }
    public string? Color { get; init; }
    public decimal? Price { get; init; }
    public int? Mileage { get; init; }

    public string? PhotoExtension { get; init; }
    public byte[]?  PhotoData      { get; init; }
    public bool     OnlyIfNonePhoto { get; init; }

    public Guid?   NewManagerId { get; init; }
    public Role[]? NewManagerRoles { get; init; }

    public Guid EmployerId { get; init; }
    public IReadOnlyList<Role> EmployerRoles { get; init; } = Array.Empty<Role>();
}