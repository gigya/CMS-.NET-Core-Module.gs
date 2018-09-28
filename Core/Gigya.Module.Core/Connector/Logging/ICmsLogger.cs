using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Logging
{
    public interface ICmsLogger
    {
        void Write(string message, Exception exception, LogCategory category);
    }
}
