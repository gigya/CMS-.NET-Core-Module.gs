using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.SiteSettings;

namespace Gigya.Sitefinity.Module.DeleteSync.BasicSettings
{
    interface IGigyaSettingsDataContract : ISettingsDataContract
    {
        void Load();
        void Save();
    }
}
