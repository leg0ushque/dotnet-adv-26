namespace Ecommerce.CartService.BusinessLogic.Results
{
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

        public static Error NotFound(string entityName, string id) =>
            new($"{entityName}.NotFound", $"{entityName} with id '{id}' was not found", ErrorType.NotFound);

        public static Error Validation(string code, string message) =>
            new(code, message, ErrorType.Validation);

        public static Error Conflict(string code, string message) =>
            new(code, message, ErrorType.Conflict);

        public static Error Failure(string code, string message) =>
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

}