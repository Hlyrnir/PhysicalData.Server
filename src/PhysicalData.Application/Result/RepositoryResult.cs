namespace PhysicalData.Application.Result
{
    public enum RepositoryResultState : byte
    {
        Failure,
        Success
    }

    public readonly struct RepositoryResult<T>
        where T : notnull
    {
        private readonly RepositoryResultState enumState;
        private readonly T? gValue;

        private readonly RepositoryError? msgError;

        public RepositoryResult(T gValue)
        {
            enumState = RepositoryResultState.Success;
            this.gValue = gValue;
            msgError = null;
        }

        public RepositoryResult(RepositoryError msgError)
        {
            enumState = RepositoryResultState.Failure;
            gValue = default;
            this.msgError = msgError;
        }

        public static implicit operator RepositoryResult<T>(T gValue)
        {
            return new RepositoryResult<T>(gValue);
        }

        public bool IsFailed
        {
            get
            {
                if (enumState == RepositoryResultState.Failure)
                    return true;

                return false;
            }
        }

        public bool IsSuccess
        {
            get
            {
                if (enumState == RepositoryResultState.Success)
                    return true;

                return false;
            }
        }

        public R Match<R>(Func<RepositoryError, R> MethodIfIsFailed, Func<T, R> MethodIfIsSuccess)
        {
            if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
                throw new NotImplementedException("Match function is not defined.");

            if (gValue is null && msgError is null)
                throw new InvalidOperationException("No result was found.");

            if (enumState == RepositoryResultState.Success)
                return MethodIfIsSuccess(gValue!);

            return MethodIfIsFailed((RepositoryError)msgError!);
        }

        public async Task<R> MatchAsync<R>(Func<RepositoryError, R> MethodIfIsFailed, Func<T, Task<R>> MethodIfIsSuccess)
        {
            if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
                throw new NotImplementedException("Match function is not defined.");

            if (gValue is null && msgError is null)
                throw new InvalidOperationException("No result was found.");

            if (enumState == RepositoryResultState.Success)
                return await MethodIfIsSuccess(gValue!);

            return MethodIfIsFailed((RepositoryError)msgError!);
        }
    }
}
