namespace CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;

public enum CartAction
{
    Success,
    
    ErrorCartAlreadyOrdered,
    
    ErrorCarAlreadyInCart,
    ErrorCarIsNotAvailable,
    ErrorCarNotInCart,
}

public enum OrderCartAction
{
    Success,
    
    ErrorAlreadyOrdered,

    ErrorCartEmpty,
    ErrorCarsMismatch,
    ErrorSomeCarIsNotAvailable,
}

public enum PurchaseCartAction
{
    Success,
    
    ErrorCartNotOrdered,
    ErrorNotReadyForPurchase,
    ErrorAlreadyPurchased,
    ErrorCarsMismatch,
}