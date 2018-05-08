using System.Web.Mvc;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;

namespace Gigya.Module.Core.Connector.Helpers
{
    public interface IGigyaSettingsHelper
    {
        string CmsName { get; }
        string CmsVersion { get; }
        string ModuleVersion { get; }
        IPathUtilities PathUtilities { set; }

        void DecryptApplicationSecret(ref IGigyaModuleSettings settings);
        void Delete(object id);
        IGigyaModuleSettings Get(object id, bool decrypt = false);
        IGigyaModuleSettings GetForCurrentSite(bool decrypt = false);
        int SessionExpiration(IGigyaModuleSettings settings);
        void Validate(IGigyaModuleSettings settings);
        GigyaSettingsViewModel ViewModel(IGigyaModuleSettings settings, UrlHelper urlHelper, CurrentIdentity currentIdentity);
    }
}