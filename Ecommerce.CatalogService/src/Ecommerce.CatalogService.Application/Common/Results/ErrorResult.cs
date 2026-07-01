namespace Ecommerce.CatalogService.Application.Common.Results;

public sealed record ErrorResult(string Code, string Message, ErrorType Type)
{
    public static readonly ErrorResult None = new(string.Empty, string.Empty, ErrorType.None);

    public static ErrorResult NotFound(string entityName, string id) =>
        new($"{entityName}.NotFound", $"{entityName} with id '{id}' was not found", ErrorType.NotFound);

    public static ErrorResult Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static ErrorResult Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static ErrorResult Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
}

public enum ErrorType
{
    None,
    NotFound,
    Validation,
    Conflict,
    Failure
}
