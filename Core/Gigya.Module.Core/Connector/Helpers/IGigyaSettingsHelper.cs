using System.Web.Mvc;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;

namespace Gigya.Module.Core.Connector.Helpers
{
    public interface IGigyaSettingsHelper<T> where T : IGigyaModuleSettings, new()
    {
        string CmsName { get; }
        string CmsVersion { get; }
        string ModuleVersion { get; }
        IPathUtilities PathUtilities { set; }

        void DecryptApplicationSecret(ref T settings);
        void Delete(object id);
        T Get(object id, bool decrypt = false);
        T GetForCurrentSite(bool decrypt = false);
        int SessionExpiration(T settings);
        void Validate(T settings);
        GigyaSettingsViewModel ViewModel(T settings, UrlHelper urlHelper, CurrentIdentity currentIdentity);
    }
}