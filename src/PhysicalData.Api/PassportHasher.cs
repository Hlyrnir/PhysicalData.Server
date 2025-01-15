using Microsoft.Extensions.Options;
using Passport.Abstraction.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace PhysicalData.Api
{
    public class PassportHasher : IPassportHasher
    {
        PassportHashSetting ppHashSetting;

        public PassportHasher(IOptions<PassportHashSetting> optAccessor)
        {
            ppHashSetting = optAccessor.Value;
        }

        public byte[] HashSignature(string sUnprotectedSignature)
        {
            using (HMACSHA256 hmacSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(ppHashSetting.PublicKey)))
            {
                return hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(sUnprotectedSignature));
            }
        }
    }
}
