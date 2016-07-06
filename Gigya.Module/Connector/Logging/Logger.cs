using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;
using T = Telerik.Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Gigya.Module.Connector.Logging
{
    public static class Logger
    {
        private const string _messagePrefix = "[Gigya]: ";

        public static void Debug(string message)
        {
            Log(message, ConfigurationPolicy.Debug, null);
        }

        public static void DebugFormat(string message, params object[] values)
        {
            Log(string.Format(message, values), ConfigurationPolicy.Debug, null);
        }

        public static void Error(string message)
        {
            Log(message, ConfigurationPolicy.ErrorLog, null);
        }

        public static void Error(string message, Exception e)
        {
            Log(message, ConfigurationPolicy.ErrorLog, e);
        }

        public static void Warn(string message)
        {
            Log(message, ConfigurationPolicy.Trace, null);
        }

        public static void Warn(string message, Exception e)
        {
            Log(message, ConfigurationPolicy.Trace, e);
        }

        private static void Log(string message, ConfigurationPolicy category, Exception exception)
        {
            if (exception != null)
            {
                message = string.Join("\nException:\n", message, exception);
            }
            T.Logger.Write(string.Concat(_messagePrefix, message), category.ToString());
        }

        public static string FormatMessage(string apiCall, string userEmail, string gigyaError)
        {
            return string.Format("Date: {0}, API call: {1}, Email: {2}, Error: {3}", DateTime.Now, apiCall, userEmail, gigyaError);
        }
    }
}
