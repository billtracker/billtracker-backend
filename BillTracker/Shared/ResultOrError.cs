using System;

namespace BillTracker.Shared
{
    public class ResultOrError<TResult, TError>
    {
        protected ResultOrError(TResult result)
        {
            Result = result;
            IsError = false;
        }

        protected ResultOrError(TError error)
        {
            Error = error;
            IsError = true;
        }

        public bool IsError { get; }
        public TError Error { get; }
        public TResult Result { get; }

        public void Match(Action<TResult> onResult, Action<TError> onError)
        {
            if (this.IsError)
            {
                onError(this.Error);
            }
            else
            {
                onResult(this.Result);
            }
        }

        public T Match<T>(Func<TResult, T> resultFunc, Func<TError, T> errorFunc) => this.IsError
            ? errorFunc(this.Error)
            : resultFunc(this.Result);

        public static ResultOrError<TResult, TError> FromResult(TResult result) => new ResultOrError<TResult, TError>(result);
        public static ResultOrError<TResult, TError> FromError(TError error) => new ResultOrError<TResult, TError>(error);
    }

    public class ResultOrError<TResult> : ResultOrError<TResult, string>
    {
        protected ResultOrError(TResult result) : base(result) { }

        protected ResultOrError(string error) : base(error) { }

        public new static ResultOrError<TResult> FromResult(TResult result) => new ResultOrError<TResult>(result);
        public new static ResultOrError<TResult> FromError(string error) => new ResultOrError<TResult>(error);

        public static implicit operator ResultOrError<TResult>(TResult result) => FromResult(result);
        public static implicit operator ResultOrError<TResult>(string error) => FromError(error);
    }

    public class SuccessOrError : ResultOrError<bool>
    {
        protected SuccessOrError() : base(true) { }

        protected SuccessOrError(string error) : base(error) { }

        public static SuccessOrError FromSuccess() => new SuccessOrError();
        public new static SuccessOrError FromError(string error) => new SuccessOrError(error);

        public static implicit operator SuccessOrError(string error) => FromError(error);
    }
}
