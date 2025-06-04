namespace CarDDD.DomainServices.DomainAggregates.CartAggregate.Results;

public enum CartAction
{
    Success,
    
    ErrorCartAlreadyOrdered,
    
    ErrorCarAlreadyInCart,
    ErrorCarNotInCart,
}

public enum OrderCartAction
{
    Success,
    
    ErrorAlreadyOrdered,

    ErrorCartEmpty,
}

public enum PurchaseCartAction
{
    Success,
    
    ErrorCartNotOrdered,
    ErrorNotReadyForPurchase,
    ErrorAlreadyPurchased,
    ErrorCarsMismatch,
}