using CarDDD.ApplicationServices.Models.AnswerObjects.Result;
using CarDDD.ApplicationServices.Models.RequestModels;

namespace CarDDD.ApplicationServices.Services.Helpers;

public static class CarRequestValidator
{
    public static Result<bool> Validate(AddNewCarRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Brand))
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Brand null or empty"));
        
        if (string.IsNullOrWhiteSpace(req.Color))
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Color null or empty"));
        
        if (req.Price.Equals(decimal.Zero))
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Price is zero"));
        
        if (req.EmployerId == Guid.Empty || req.EmployerRoles.Count == 0)
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "Employee id or roles is empty"));
        
        if (!string.IsNullOrWhiteSpace(req.PhotoExtension) && req.PhotoExtension.Length == 0)
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "PhotoExtension exist but photo data is empty"));

        if (req.PhotoExtension.Length > 0 && string.IsNullOrWhiteSpace(req.PhotoExtension))
            return Result<bool>.Failure(Error.Application(ErrorType.Validation, "PhotoData exist but photo extension is empty"));

        return Result<bool>.Success(true);
    }
}