using Gigya.Module.Core.Connector.Events;
using Gigya.Module.DS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS
{
    public static class ModuleInstaller
    {
        public static void PreApplicationStart()
        {
            // not required as event must be attached in by CMS specific module rather than core code
        }
    }
}
