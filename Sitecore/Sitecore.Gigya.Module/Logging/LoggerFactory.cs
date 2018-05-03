using Gigya.Module.Core.Connector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Module.Logging
{
    public static class LoggerFactory
    {
        public static Logger Instance()
        {
            return new Logger(new SitecoreLogger());
        }
    }
}
