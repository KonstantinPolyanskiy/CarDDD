namespace CarDDD.ApplicationServices.Models.AnswerObjects.Result;

public enum ErrorLayer
{
    Application,
    Domain,
    Infrastructure
}

public enum ErrorType
{
    NotFound,
    Forbidden,
    Validation,
    Conflict,
    Unknown
}

public sealed record Error(ErrorLayer Layer, ErrorType Type, string Message)
{
    public static Error Application(ErrorType type, string message) =>
        new(ErrorLayer.Application, type, message);

    public static Error Domain(ErrorType type, string message) =>
        new(ErrorLayer.Domain, type, message);
    
    public static Error Infrastructure(ErrorType type, string message) =>
        new(ErrorLayer.Infrastructure, type, message);
}