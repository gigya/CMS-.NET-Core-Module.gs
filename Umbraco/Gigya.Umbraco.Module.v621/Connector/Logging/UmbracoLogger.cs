using Gigya.Module.Core.Connector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;

namespace Gigya.Umbraco.Module.v621.Connector
{
    public class UmbracoLogger : ICmsLogger
    {
        private log4net.ILog _logger = LogManager.GetLogger("GigyaUmbracoLogger");

        public void Write(string message, Exception exception, LogCategory category)
        {
            switch(category)
            {
                case LogCategory.Debug:
                    _logger.Debug(message, exception);
                    return;
                case LogCategory.Trace:
                    _logger.Info(message, exception);
                    return;
                case LogCategory.Error:
                default:
                    _logger.Error(message, exception);
                    return;
            }
        }
    }
}
