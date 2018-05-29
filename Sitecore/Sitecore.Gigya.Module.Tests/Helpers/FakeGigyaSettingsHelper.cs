using Sitecore.Gigya.Module.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.Module.Tests.Helpers
{
    public class FakeGigyaSettingsHelper : GigyaSettingsHelper
    {
        protected override string ReadVersionFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "sitecore/shell/sitecore.version.xml");
            var versionInfo = File.ReadAllText(path);
            return versionInfo;
        }
    }
}
