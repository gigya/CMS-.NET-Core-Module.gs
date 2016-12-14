using Gigya.Module.Core.Connector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Umbraco.Module.Connector.Logging
{
    public static class LoggerFactory
    {
        public static Logger Instance()
        {
            return new Logger(new UmbracoLogger());
        }
    }
}