using System.Security.Claims;
using CarDDD.Core.AnswerObjects.Result;
using CarDDD.Core.DomainObjects.CommonValueObjects;
using CarDDD.Core.DomainObjects.DomainCar;
using CarDDD.Core.DomainObjects.DomainCar.Actions;
using CarDDD.Core.DomainObjects.DomainCar.EntityObjects;
using CarDDD.Core.DomainObjects.DomainCart;
using CarDDD.Core.DomainObjects.DomainCart.Actions;
using CarDDD.Core.EntityObjects;
using CarDDD.Infrastructure.EventDispatchers.DomainDispatchers;
using CarDDD.Infrastructure.Repositories;
using Car = CarDDD.Core.DomainObjects.DomainCart.Car;

namespace CarDDD.Infrastructure.Services;

//TODO: все заменить на claims пользователя
public class CartService(ICarRepository cars, ICartRepository carts, IDomainEventDispatcher dispatcher)
{
    public async Task<Result<bool>> AddCarAsync(Guid carId, ClaimsPrincipal user)
    {
        var cart = await carts.GetByIdAsync(Guid.NewGuid());
        if (cart == null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "Cart not found"));
        
        var car = await cars.FindByIdAsync(carId);
        if (car == null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "Adding car not found"));

        var cartResult = cart.AddCar(new Car(CarId.From(car.EntityId), car.IsAvailable));
        if (cartResult.Status is not CartAction.Success)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart adding failed"));

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveCarAsync(Guid carId, ClaimsPrincipal user)
    {
        var cart = await carts.GetByIdAsync(carId);
        if (cart == null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "Cart not found"));

        var cartResult = cart.RemoveCar(new Car(CarId.From(carId)));
        if (cartResult.Status is not CartAction.Success)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart removing failed"));
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PurchaseAsync(Consumer buyer)
    {
        var cart = await carts.GetByIdAsync(buyer.EntityId);
        if (cart is null)
            return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "Cart not found"));

        foreach (var cartCar in cart.Cars)
        {
            var car = await cars.FindByIdAsync(cartCar.CarId.Value); 
            if (car == null)
                return Result<bool>.Failure(Error.Application(ErrorType.NotFound, "Car not found"));
            
            var sell = car.Sell(car.Price, buyer);
            if (sell.Status is not SellCarAction.Success)
                return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Car selling failed"));

            await cars.UpdateCarAsync(car);
        }

        var mark = cart.MarkPurchased(ConsumerId.From(buyer.EntityId));
        if (mark.Status is not CartAction.Success)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Car marking as sold failed"));
        
        await dispatcher.DispatchAsync(cart.DomainEvents);
        cart.ClearAllDomainEvents();

        return Result<bool>.Success(true);
    }
}