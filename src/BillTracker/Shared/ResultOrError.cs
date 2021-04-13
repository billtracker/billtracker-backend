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

        public static ResultOrError<TResult, TError> FromResult(TResult result) => new ResultOrError<TResult, TError>(result);

        public static ResultOrError<TResult, TError> FromError(TError error) => new ResultOrError<TResult, TError>(error);

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
    }

    public class ResultOrError<TResult> : ResultOrError<TResult, string>
    {
        protected ResultOrError(TResult result)
            : base(result)
        {
        }

        protected ResultOrError(string error)
            : base(error)
        {
        }

        public static implicit operator ResultOrError<TResult>(TResult result) => FromResult(result);

        public static implicit operator ResultOrError<TResult>(string error) => FromError(error);

        public static new ResultOrError<TResult> FromResult(TResult result) => new ResultOrError<TResult>(result);

        public static new ResultOrError<TResult> FromError(string error) => new ResultOrError<TResult>(error);
    }

    public class SuccessOrError : ResultOrError<bool>
    {
        protected SuccessOrError()
            : base(true)
        {
        }

        protected SuccessOrError(string error)
            : base(error)
        {
        }

        public static implicit operator SuccessOrError(string error) => FromError(error);

        public static SuccessOrError FromSuccess() => new SuccessOrError();

        public static new SuccessOrError FromError(string error) => new SuccessOrError(error);
    }
}
