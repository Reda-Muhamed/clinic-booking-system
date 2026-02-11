namespace ClinicBookingSystem.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        protected Result(bool isSuccess, string? errorMessage)
        {
            if (isSuccess && errorMessage != null)
                throw new InvalidOperationException("Success result cannot have an error message.");

            if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
                throw new InvalidOperationException("Failure result must have an error message.");

            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new Result(true, null);

        public static Result Failure(string errorMessage) =>
            new Result(false, errorMessage);
    }
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? errorMessage)
            : base(isSuccess, errorMessage)
        {
            if (isSuccess && value == null)
                throw new InvalidOperationException("Success result must have a value.");

            if (!isSuccess && value != null)
                throw new InvalidOperationException("Failure result cannot have a value.");

            Value = value;
        }

        public static Result<T> Success(T value) =>
            new Result<T>(true, value, null);

        public new static Result<T> Failure(string errorMessage) =>
            new Result<T>(false, default, errorMessage);
    }
}
