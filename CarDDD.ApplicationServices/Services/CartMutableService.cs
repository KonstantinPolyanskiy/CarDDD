using CarDDD.ApplicationServices.Dispatchers;
using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Repositories;
using CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;
using CarDDD.DomainServices.Services;
using CarDDD.DomainServices.Specifications;
using CarDDD.DomainServices.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Car = CarDDD.DomainServices.Specifications.Car;

namespace CarDDD.ApplicationServices.Services;

public interface ICartMutableService
{
    public Task<Result<bool>> AddCarToCart(Guid customerId, Guid carId, CancellationToken ct = default);
    public Task<Result<bool>> RemoveCarFromCart(Guid customerId, Guid carId, CancellationToken ct = default);
    public Task<Result<bool>> OrderCart(Guid customerId, CancellationToken ct = default);
    public Task<Result<bool>> PurchaseCart(Guid customerId, CancellationToken ct = default);
}

/// <summary>
/// Сервис для изменения состояния корзины и машин
/// </summary>
public class CartMutableService(ICartDomainService cartService, ICartRepository cartRepository, 
    ICarRepositoryReader carReader, IDomainEventDispatcher dispatcher) : ICartMutableService 
{
    public async Task<Result<bool>> AddCarToCart(Guid customerId, Guid carId, CancellationToken ct = default)
    {
        // Находим корзину пользователя, если ее нет - создаем
        var cart = await cartRepository.GetAsync(CustomerId.From(customerId), ct)
                   ?? cartService.CreateCart(new CreateCartSpec { Customer = CustomerId.From(customerId) });
        
        // Находим добавляемую машину
        var car = await carReader.GetAsync(CarId.From(carId), ct);
        if (car == null)
            return Result<bool>.Failure(Error.Domain(ErrorType.NotFound, "Adding car not found"));

        // Добавляем машину в корзину
        var added = cartService.AddCar(cart, new AddCarCartSpec { Car = new Car(CarId.From(car.EntityId), car.IsAvailable) });
        if (added.Status is not CartAction.Success)
        {
            switch (added.Status)
            {
                case CartAction.ErrorCartAlreadyOrdered:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart already ordered"));
                case CartAction.ErrorCarAlreadyInCart:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart already in cart"));
            }
        }

        // Сохраняем корзину
        var saved = await cartRepository.UpdateAsync(cart, ct);
        if (!saved)
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.Unknown, "Something went wrong, cart not saved"));

        await dispatcher.DispatchAsync(cart.DomainEvents, ct);
        cart.ClearAllDomainEvents();
        
        return Result<bool>.Success(saved);
    }

    public async Task<Result<bool>> RemoveCarFromCart(Guid customerId, Guid carId, CancellationToken ct = default)
    {
        // Находим корзину пользователя
        var cart = await cartRepository.GetAsync(CustomerId.From(customerId), ct);
        if (cart == null)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart not found"));
        
        // Удаляем из нее машину
        var removed = cartService.RemoveCar(cart, new RemoveCarCartSpec { Car = new Car(CarId.From(carId), true ) });
        if (removed.Status is not CartAction.Success)
        {
            switch (removed.Status)
            {
                case CartAction.ErrorCartAlreadyOrdered:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart already ordered"));
                case CartAction.ErrorCarNotInCart:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart is not in cart"));
            }
        }
        
        // Обновляем корзину в хранилище
        var saved = await cartRepository.UpdateAsync(cart, ct);
        if (!saved)
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.Unknown, "Something went wrong, cart not saved"));
        
        await dispatcher.DispatchAsync(cart.DomainEvents, ct);
        cart.ClearAllDomainEvents();
        
        return Result<bool>.Success(saved);
    }

    public async Task<Result<bool>> OrderCart(Guid customerId, CancellationToken ct = default)
    {
        // Находим корзину пользователя
        var cart = await cartRepository.GetAsync(CustomerId.From(customerId), ct);
        if (cart == null)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart not found"));
        
        // Находим заказываемые машины
        var allCarsQuery = await carReader.CarsQueryAsync();
        var allCars = await allCarsQuery.ToListAsync(ct);

        var carsToOrder = allCars
            .Where(c => cart.Cars.Contains(new DomainServices.DomainAggregates.CartAggregate.Car(CarId.From(c.EntityId))))
            .Select(c => new Car(CarId.From(c.EntityId), c.IsAvailable))
            .ToList();
        if (!carsToOrder.Any())
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "cars is empty"));
        
        // Заказываем корзину
        var ordered = cartService.OrderCart(cart, 
            new OrderCartSpec
            {
                Cars = carsToOrder
            });
        if (ordered.Status is not OrderCartAction.Success)
        {
            switch (ordered.Status)
            {
                case OrderCartAction.ErrorAlreadyOrdered:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart already ordered"));
                case OrderCartAction.ErrorCartEmpty:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart is empty"));
                case OrderCartAction.ErrorCarsMismatch:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Some car in cart is not in cart"));
                case OrderCartAction.ErrorSomeCarIsNotAvailable:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Some car is cart is not available"));
            }
        }
        
        // Обновляем данные корзины
        var cartSaved = await cartRepository.UpdateAsync(cart, ct);
        if (!cartSaved)
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.Unknown, "Something went wrong, cart not saved"));
        
        await dispatcher.DispatchAsync(cart.DomainEvents, ct);
        cart.ClearAllDomainEvents();
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PurchaseCart(Guid customerId, CancellationToken ct = default)
    {
        // Находим корзину
        var cart = await cartRepository.GetAsync(CustomerId.From(customerId), ct);
        if (cart == null)
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart not found"));
        
        var allCarsQuery = await carReader.CarsQueryAsync();
        var allCars = await allCarsQuery.ToListAsync(ct);

        var carsToSell = allCars
            .Where(c => cart.Cars.Contains(new DomainServices.DomainAggregates.CartAggregate.Car(CarId.From(c.EntityId))))
            .Select(c => new Car(CarId.From(c.EntityId), c.IsAvailable))
            .ToList();
        if (!carsToSell.Any())
            return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "cars is empty"));

        
        // Выкупаем корзину
        var purchased = cartService.PurchaseCart(
            cart,
            new PurchaseCartSpec
            {
                CarsToSell = carsToSell
            });
        if (purchased.PurchaseResult.Status is not PurchaseCartAction.Success)
        {
            switch (purchased.PurchaseResult.Status)
            {
                case PurchaseCartAction.ErrorCartNotOrdered:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart is not ordered"));
                case PurchaseCartAction.ErrorNotReadyForPurchase:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart is not ready to purchase"));
                case PurchaseCartAction.ErrorAlreadyPurchased:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Cart is already purchased"));
                case PurchaseCartAction.ErrorCarsMismatch:
                    return Result<bool>.Failure(Error.Domain(ErrorType.Conflict, "Some car is cart is mismatch"));
            }
        }
        
        // Обновляем корзину
        var cartSaved = await cartRepository.UpdateAsync(cart, ct);
        if (!cartSaved)
            return Result<bool>.Failure(Error.Infrastructure(ErrorType.Unknown, "Something went wrong, cart not updated"));
        
        await dispatcher.DispatchAsync(cart.DomainEvents, ct);
        cart.ClearAllDomainEvents();

        return Result<bool>.Success(true);
    }
}