using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.SiteSettings;

namespace Gigya.Sitefinity.Module.DS.BasicSettings
{
    interface IGigyaDSSettingsDataContract : ISettingsDataContract
    {
        Guid SiteId { get; }
        void Load(Guid id);
        void Save(Guid id);
    }
}
