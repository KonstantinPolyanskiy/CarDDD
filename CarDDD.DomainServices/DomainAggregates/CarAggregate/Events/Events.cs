using CarDDD.DomainServices.BaseDomainEvents;
using CarDDD.DomainServices.ValueObjects;

namespace CarDDD.DomainServices.DomainAggregates.CarAggregate.Events;

/// <summary> Машина создана </summary>
public sealed record CarCreated(CarId CarId, EmployerId ManagerId, bool HasPhoto) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary> Машина продана </summary>
public sealed class CarSold(CarId carId, EmployerId ManagerId, CustomerId CustomerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary> Машина создана без фото </summary>
public sealed record CarCreatedWithoutPhoto(CarId CarId, EmployerId ManagerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary> Обновлены базовые аттрибуты машины </summary>
public sealed record CarUpdatedBasisAttributes(CarId CarId, EmployerId EmployerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary> Обновлен ответственный менеджер машины </summary>
public sealed class CarManagerChanged(CarId carId, EmployerId NewManagerId, EmployerId OldManagerId, EmployerId AdminId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary> Прикреплено фото к машине </summary>
public sealed record CarPhotoAttached(CarId CarId, PhotoId PhotoId, EmployerId EmployerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

