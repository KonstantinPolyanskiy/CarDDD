namespace CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;

/// <summary> Результат создания агрегата <see cref="Car"/> </summary>
public record CreateCarResult(CreateCarAction Status, Car Car);

/// <summary> Результат обновления агрегата <see cref="Car"/> </summary>
public record UpdateCarResult(UpdateCarAction Status);

/// <summary> Результат продажи агрегата <see cref="Car"/> </summary>
public record SellCarResult(SellCarAction Status);