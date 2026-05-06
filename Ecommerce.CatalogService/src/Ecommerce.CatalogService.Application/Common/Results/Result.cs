namespace Ecommerce.CatalogService.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, Error? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Success result cannot have an error");
            
            if (!isSuccess && error == null)
                throw new InvalidOperationException("Failure result must have an error");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);
        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value => IsSuccess 
            ? _value! 
            : throw new InvalidOperationException("Cannot access value of a failed result");

        internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
        {
            _value = value;
        }
    }
}
