namespace CarDDD.Core.DomainObjects.DomainCart.Actions;

public enum CartAction
{
    Success,
    ErrorAlreadyCheckedOut,
    ErrorCarAlreadyAdded,
    ErrorCarNotInCart,
    ErrorCarNotAvailable,
}