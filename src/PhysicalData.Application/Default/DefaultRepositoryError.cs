using PhysicalData.Application.Result;

namespace PhysicalData.Application.Default
{
    public static class DefaultRepositoryError
    {
        public static RepositoryError TaskAborted => new RepositoryError() { Code = "TASK_ABORTED", Description = "Task has been cancelled." };
        public static RepositoryError ConcurrencyViolation => new RepositoryError() { Code = Code.Method, Description = "Data has been modified. Refresh and try again." };
        
        public static class Code
        {
            public static string Method = "METHOD_REPOSITORY";
            public static string Exception = "EXCEPTION_REPOSITORY";
        }
    }

    public static class PhysicalDimensionError
    {
        public static class Code
        {
            public static string Method = "METHOD_REPOSITORY_PHYSICAL_DIMENSION";
            public static string Exception = "EXCEPTION_REPOSITORY_PHYSICAL_DIMENSION";
        }
    }

    public static class TimePeriodError
    {
        public static class Code
        {
            public static string Method = "METHOD_REPOSITORY_TIME_PERIOD";
            public static string Exception = "EXCEPTION_REPOSITORY_TIME_PERIOD";
        }
    }
}