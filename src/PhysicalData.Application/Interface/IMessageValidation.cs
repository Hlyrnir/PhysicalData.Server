using Passport.Abstraction.Result;

namespace PhysicalData.Application.Interface
{
    public interface IMessageValidation
    {
        bool IsValid { get; }

        bool Add(IMessageError msgError);
        R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess);
        bool ValidateAgainstSqlInjection(string sString, string sPropertyName);
        bool ValidateGuid(Guid guGuid, string sPropertyName);
    }
}