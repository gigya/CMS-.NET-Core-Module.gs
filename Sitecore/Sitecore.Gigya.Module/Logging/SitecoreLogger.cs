using Gigya.Module.Core.Connector.Logging;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = Sitecore;

namespace Sitecore.Gigya.Module.Logging
{
    public class SitecoreLogger : ICmsLogger
    {
        private readonly Lazy<ILog> _logger = new Lazy<ILog>(() => SC.Diagnostics.LoggerFactory.GetLogger(Constants.Logging.Name));

        public void Write(string message, Exception exception, LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Debug:
                    _logger.Value.Debug(message);
                    return;
                case LogCategory.Trace:
                    _logger.Value.Info(message);
                    return;
                case LogCategory.Error:
                default:
                    _logger.Value.Error(message, exception);
                    return;
            }
        }
    }
}
