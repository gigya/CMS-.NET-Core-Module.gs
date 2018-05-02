using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.Core.Connector.Helpers
{
    public interface IPathUtilities
    {
        string ToAbsolute(string virtualPath);
    }
}
