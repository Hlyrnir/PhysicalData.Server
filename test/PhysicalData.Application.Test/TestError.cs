using PhysicalData.Application.Result;

namespace PhysicalData.Application.Test
{
    internal static class TestError
    {
        internal static class Repository
        {
            internal static class Domain
            {
                internal static RepositoryError NotInitialized = new RepositoryError() { Code = "TEST_METHOD_DOMAIN", Description = "Transfer object could not be initialized." };
            }

            internal static class PhysicalDimension
            {
                internal static class Code
                {
                    public const string Method = "TEST_METHOD_PHYSICAL_DIMENSION";
                }

                public static RepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Physical dimension does already exist in repository." };
                public static RepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Physical dimension does not exist in repository." };
            }

            internal static class TimePeriod
            {
                internal static class Code
                {
                    public const string Method = "TEST_METHOD_TIME_PERIOD";
                }

                public static RepositoryError Exists = new RepositoryError() { Code = Code.Method, Description = "Time period does already exist in repository." };
                public static RepositoryError NotFound = new RepositoryError() { Code = Code.Method, Description = "Time period does not exist in repository." };
            }
        }
    }
}
