using PhysicalData.Application.Result;

namespace PhysicalData.Application.Default
{
    public static class DefaultMessageError
    {
        public static MessageError TaskAborted = new MessageError() { Code = "TASK_ABORTED", Description = "Task has been cancelled." };
        public static MessageError ConcurrencyViolation = new MessageError() { Code = "METHOD_APPLICATION", Description = "Data has been modified. Refresh and try again." };
    }

    internal static class ValidationError
    {
        internal static class Code
        {
            public static string Method = "METHOD_VALIDATION";
        }

        internal static class PhysicalDimension
        {
            public static MessageError IsInvalid = new MessageError() { Code = Code.Method, Description = "Validation rules have been violated." };
        }
    }

    internal static class DomainError
    {
        internal static class Code
        {
            public static string Method = "METHOD_DOMAIN";
        }

        public static MessageError InitializationHasFailed = new MessageError() { Code = Code.Method, Description = "Transfer object could not be initialized." };
    }
}
