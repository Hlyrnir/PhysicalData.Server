using Passport.Abstraction.Result;
using PhysicalData.Application.Default;
using PhysicalData.Application.Interface;
using PhysicalData.Application.Result;
using System.Text.Json;

namespace PhysicalData.Application.Validation
{
    public class MessageValidation : IMessageValidation
    {
        private List<IMessageError> lstValidationError;

        public MessageValidation()
        {
            lstValidationError = new List<IMessageError>();
        }

        public bool IsValid
        {
            get
            {
                if (lstValidationError.Count == 0)
                    return true;

                return false;
            }
        }

        public R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess)
        {
            if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
                throw new NotImplementedException("Match function is not defined.");

            if (IsValid)
                return MethodIfIsSuccess(true);

            return MethodIfIsFailed(Summary());
        }

        public bool Add(IMessageError msgError)
        {
            if (msgError is null)
                return false;

            lstValidationError.Add(msgError);

            return true;
        }

        private IMessageError Summary()
        {
            JsonSerializerOptions jsonOption = new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            return new MessageError()
            {
                Code = ValidationError.Code.Method,
                Description = JsonSerializer.Serialize(lstValidationError, jsonOption)
            };
        }

        public bool ValidateGuid(Guid guGuid, string sPropertyName)
        {
            if (Equals(guGuid, Guid.Empty) == true)
            {
                Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"{sPropertyName} is invalid (empty)." });
                return false;
            }

            return true;
        }

        public bool ValidateAgainstSqlInjection(string sString, string sPropertyName)
        {
            if (sString.Contains("--", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("ALTER", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("DELETE", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("DROP", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("INSERT", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("SELECT", StringComparison.InvariantCultureIgnoreCase) == true
                || sString.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"{sPropertyName} contains forbidden statement." });
                return false;
            }

            return true;
        }
    }
}
