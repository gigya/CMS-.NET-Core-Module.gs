using Gigya.Module.Core.Connector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telerik.Sitefinity.Abstractions;
using T = Telerik.Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Gigya.Module.Connector.Logging
{
    public class SitefinityLogger : ICmsLogger
    {
        public void Write(string message, Exception exception, LogCategory category)
        {
            switch(category)
            {
                case LogCategory.Debug:
                    Log(message, ConfigurationPolicy.Debug, exception);
                    return;
                case LogCategory.Trace:
                    Log(message, ConfigurationPolicy.Trace, exception);
                    return;
                case LogCategory.Error:
                default:
                    Log(message, ConfigurationPolicy.ErrorLog, exception);
                    return;
            }
        }

        private void Log(string message, ConfigurationPolicy category, Exception exception)
        {
            if (exception != null)
            {
                message = string.Join("\nException:\n", message, exception);
            }
            T.Logger.Write(message, category.ToString());
        }
    }
}
