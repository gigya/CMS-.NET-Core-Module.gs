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
        public void Write(string message, Exception exception, LogCategory category)
        {
        }
    }
}
