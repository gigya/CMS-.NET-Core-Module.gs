using Gigya.Module.Core.Connector.Extensions;
using Gigya.Socialize.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Helpers
{
    public class GigyaSignatureHelpers
    {
        public static string GetDynamicSessionSignatureUserSigned(string gigyaAuthCookie, int timeoutInSeconds, string applicationKey, string secret)
        {
            var epoch = DateTime.UtcNow.DateTimeToUnixTimestamp();
            string expirationTime = (epoch + timeoutInSeconds).ToString();
            string signature = SigUtils.CalcSignature(gigyaAuthCookie + "_" + expirationTime + "_" + applicationKey, secret);
            return expirationTime + '_' + applicationKey + "_" + signature;
        }
    }
}
