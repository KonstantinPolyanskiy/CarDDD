using CarDDD.Core.DomainObjects.DomainCar.Actions;

namespace CarDDD.Core.DomainObjects.DomainCar.Results;

public record CreateCarResult(CreateCarAction Status, Car Car);

public record UpdateCarResult(UpdateCarAction Status);

public record SellCarResult(SellCarAction Status);