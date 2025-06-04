namespace CarDDD.DomainServices.DomainAggregates.CarAggregate.Results;

public enum CreateCarAction
{
    Success,
    
    ErrorInvalidPrice,
    ErrorAttachPhoto,
}

public enum UpdateCarAction
{
    Success,
    
    ErrorInvalidPrice,
    ErrorEnoughPermission,
    
    ErrorPhotoAlreadyExists,
    ErrorPhotoNotAttached,
    ErrorCreatingPhoto,
}

public enum SellCarAction
{
    Success,
    
    ErrorIsNotAvailable,
    ErrorInvalidPrice,
}