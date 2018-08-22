using Gigya.Module.Core.Connector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DeleteSync.Tests.Logging
{
    public class FakeCmsLogger : ICmsLogger
    {
        public StringBuilder Logs = new StringBuilder();

        public void Write(string message, Exception exception, LogCategory category)
        {
            Log(message, exception);
        }

        private void Log(string message, Exception exception)
        {
            if (exception != null)
            {
                message = string.Join("\nException:\n", message, exception);
            }
            Logs.AppendLine(message);
        }
    }
}
