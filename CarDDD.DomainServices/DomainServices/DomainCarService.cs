using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.Results;
using CarDDD.DomainServices.Specifications;

namespace CarDDD.DomainServices.DomainServices;

public class DomainCarService
{
    public CreateCarResult CreateCar(CreateCarSpec s)
    {
        return Car.Create(
            brand: s.Brand,
            color: s.Color,
            price: s.Price,
            previousOwner: s.Ownership,
            photo: s.Photo,
            manager: s.Manager);
    }
        
    
}