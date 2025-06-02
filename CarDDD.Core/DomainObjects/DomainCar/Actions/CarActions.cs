namespace CarDDD.Core.DomainObjects.DomainCar.Actions;

public enum CreateCarAction
{
    Success,
    
    ErrorInvalidPrice,
}

public enum UpdateCarAction
{
    Success,
    
    ErrorEnoughPermission,
}

public enum SellCarAction
{
    Success,
    
    ErrorIsNotAvailable,
    ErrorInvalidPrice,
}