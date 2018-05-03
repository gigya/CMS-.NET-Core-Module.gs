using Gigya.Module.Core.Connector.Logging;
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
        public void Write(string message, Exception exception, LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Debug:
                    SC.Diagnostics.Log.Debug(message, this);
                    return;
                case LogCategory.Trace:
                    SC.Diagnostics.Log.Info(message, this);
                    return;
                case LogCategory.Error:
                default:
                    SC.Diagnostics.Log.Error(message, exception, this);
                    return;
            }
        }
    }
}
