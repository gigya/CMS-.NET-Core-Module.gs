using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Logging
{
    public class Logger
    {
        private ICmsLogger _cmsLogger;
        private const string _messagePrefix = "[Gigya]: ";

        public Logger(ICmsLogger CmsLogger)
        {
            _cmsLogger = CmsLogger;
        }

        public void Debug(string message)
        {
            Log(message, LogCategory.Debug, null);
        }

        public void DebugFormat(string message, params object[] values)
        {
            Log(string.Format(message, values), LogCategory.Debug, null);
        }

        public void Error(string message)
        {
            Log(message, LogCategory.Error, null);
        }

        public void Error(string message, Exception e)
        {
            Log(message, LogCategory.Error, e);
        }

        public void Warn(string message)
        {
            Log(message, LogCategory.Trace, null);
        }

        public void Warn(string message, Exception e)
        {
            Log(message, LogCategory.Trace, e);
        }

        private void Log(string message, LogCategory category, Exception exception)
        {
            _cmsLogger.Write(string.Concat(_messagePrefix, message), exception, category);
        }

        public string FormatMessage(string apiCall, string userEmail, string gigyaError)
        {
            return string.Format("Date: {0}, API call: {1}, Email: {2}, Error: {3}", DateTime.Now, apiCall, userEmail, gigyaError);
        }
    }
}
