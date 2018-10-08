using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Encryption;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using Gigya.Module.Core.Mvc.Models;
using Newtonsoft.Json;
using System.Dynamic;
using Gigya.Module.Core.Connector.Common;

using System.Web.Mvc;
using Gigya.Module.Core.Connector.Logging;
using System.Web;
using Gigya.Module.Core.Connector.Models;

namespace Gigya.Module.Core.Connector.Helpers
{
    public abstract class GigyaSettingsHelper : GigyaSettingsHelper<GigyaModuleSettings>, IGigyaSettingsHelper
    {
        public GigyaSettingsHelper() : base()
        {
        }
    }

    public abstract class GigyaSettingsHelper<T> : IGigyaSettingsHelper<T> where T: GigyaModuleSettings
    {
        protected IEncryptionService _encryptionService;

        public GigyaSettingsHelper() : this(EncryptionService.Instance)
        {
        }

        public GigyaSettingsHelper(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        private IPathUtilities _pathUtilities = new PathUtilities();
        public IPathUtilities PathUtilities
        {
            set { _pathUtilities = value; }
        }

        private const string _cacheKeyBase = "GigyaSettingsHelper-6A0A6E65-F65B-4197-8D4E-2BDCD5680EAF";
        protected abstract string Language(T settings);
        public abstract string CmsName { get; }
        public abstract string CmsVersion { get; }
        public abstract string ModuleVersion { get; }

        /// <summary>
        /// Gets the Gigya module settings for the current site.
        /// </summary>
        /// <param name="decrypt">Whether to decrypt the application secret.</param>
        public abstract T GetForCurrentSite(bool decrypt = false);

        /// <summary>
        /// Deletes the Gigya module settings for the site with id of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the site whose settings will be deleted.</param>
        public abstract void Delete(object id);

        protected abstract List<T> GetForSiteAndDefault(object id);

        protected abstract T EmptySettings(object id);

        protected virtual string ClientScriptPath(T settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-cms.js" : "gigya-cms.min.js";
            return string.Concat("~/scripts/", scriptName);
        }

        /// <summary>
        /// Creates a view model for use in a view.
        /// </summary>
        /// <param name="settings">The settings for the current site.</param>
        /// <param name="urlHelper">UrlHelper for the current request.</param>
        public virtual GigyaSettingsViewModel ViewModel(T settings, UrlHelper urlHelper, CurrentIdentity currentIdentity)
        {
            var model = new GigyaSettingsViewModel
            {
                ApiKey = settings.ApiKey,
                DebugMode = settings.DebugMode,
                GigyaScriptPath = UrlUtils.AddQueryStringParam(_pathUtilities.ToAbsolute(ClientScriptPath(settings, urlHelper)), "v=" + CmsVersion + ModuleVersion),
                LoggedInRedirectUrl = settings.RedirectUrl,
                LogoutUrl = settings.LogoutUrl,
                IsLoggedIn = currentIdentity.IsAuthenticated,
                Id = settings.Id,
                DataCenter = settings.DataCenter,
                IsGetInfoRequired = settings.SessionProvider == Enums.GigyaSessionProvider.CMS,
                EnableSSOToken = settings.EnableSSOToken,
                SyncSSOGroup = settings.SyncSSOGroup
            };
            
            model.Settings = !string.IsNullOrEmpty(settings.GlobalParameters) ? JsonConvert.DeserializeObject<ExpandoObject>(settings.GlobalParameters) : new ExpandoObject();

            var settingsProperties = (IDictionary<string, object>)model.Settings;
            if (!settingsProperties.ContainsKey("lang"))
            {
                model.Settings.lang = Language(settings);
            }

            if (!settingsProperties.ContainsKey("enableSSOToken"))
            {
                model.Settings.enableSSOToken = model.EnableSSOToken;
            }

            if (settings.SessionProvider == Enums.GigyaSessionProvider.Gigya && settings.GigyaSessionMode == Enums.GigyaSessionMode.Sliding)
            {
                // client needs -1 to specify a sliding session
                model.Settings.sessionExpiration = -1;
            }
            else if (settings.SessionProvider == Enums.GigyaSessionProvider.Gigya && settings.GigyaSessionMode == Enums.GigyaSessionMode.Forever)
            {
                // client needs -1 to specify a session that stays valid until the browser closes
                model.Settings.sessionExpiration = -2;
            }
            else if (settings.SessionProvider == Enums.GigyaSessionProvider.Gigya && settings.GigyaSessionMode == Enums.GigyaSessionMode.Session)
            {
                // client needs 0 to specify a session cookie
                model.Settings.sessionExpiration = 0;
            }
            else if (!settingsProperties.ContainsKey("sessionExpiration"))
            {
                model.Settings.sessionExpiration = settings.SessionTimeout;
            }

            model.SettingsJson = new HtmlString(JsonConvert.SerializeObject(model.Settings));
            return model;
        }

        public int SessionExpiration(T settings)
        {
            var globalSettings = !string.IsNullOrEmpty(settings.GlobalParameters) ? JsonConvert.DeserializeObject<ExpandoObject>(settings.GlobalParameters) : new ExpandoObject();

            var sessionExpiration = 0;
            var settingsProperties = (IDictionary<string, object>)globalSettings;
            if (settingsProperties.ContainsKey("sessionExpiration") && int.TryParse(settingsProperties["sessionExpiration"].ToString(), out sessionExpiration))
            {
                return sessionExpiration;
            }

            return settings.SessionTimeout;
        }

        /// <summary>
        /// Gets Gigya Module settings for <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The Id of the site.</param>
        /// <param name="decrypt">Whether to decrypt the application secret.</param>
        public virtual T Get(object id, bool decrypt = false)
        {
            var cacheKey = string.Concat(_cacheKeyBase, id, decrypt);
            T settings = null;

            var context = HttpContext.Current;
            if (context != null)
            {
                settings = context.Items[cacheKey] as T;
                if (settings != null)
                {
                    return settings;
                }
            }

            var siteSettingsAndGlobal = GetForSiteAndDefault(id);
            settings = GetSettingsFromGlobalOrSite(id, siteSettingsAndGlobal);

            // decrypt application secret
            if (decrypt && !string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                settings.ApplicationSecret = _encryptionService.Decrypt(settings.ApplicationSecret);
            }

            if (settings.MappedMappingFields == null)
            {
                settings.MappedMappingFields = !string.IsNullOrEmpty(settings.MappingFields)
                    ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();
            }

            MapOldDataCenter(ref settings);

            if (context != null)
            {
                // cache for the rest of this request
                context.Items[cacheKey] = settings;
            }

            return settings;
        }

        protected virtual T GetSettingsFromGlobalOrSite(object siteId, List<T> siteSettingsAndGlobal)
        {
            return siteSettingsAndGlobal.FirstOrDefault(i => i.Id == siteId) ?? siteSettingsAndGlobal.OrderByDescending(i => i.Id).FirstOrDefault() ?? EmptySettings(siteId);
        }

        private void MapOldDataCenter(ref T settings)
        {
            settings.DataCenter = MapOldDataCenter(settings.DataCenter);
        }

        public static string MapOldDataCenter(string dataCenter)
        {
            // map old DataCenters
            switch (dataCenter)
            {
                case "eu1":
                case "us1":
                case "au1":
                case "ru1":
                    return string.Concat(dataCenter, ".gigya.com");
            }

            if (!string.IsNullOrEmpty(dataCenter) && !dataCenter.Contains("."))
            {
                return string.Concat(dataCenter, ".gigya.com");
            }

            return dataCenter;
        }

        /// <summary>
        /// Decrypts an application secret if required.
        /// </summary>
        public virtual void DecryptApplicationSecret(ref T settings)
        {
            if (!string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                settings.ApplicationSecret = _encryptionService.Decrypt(settings.ApplicationSecret);
            }
        }

        public virtual string TryDecryptApplicationSecret(string secret, bool throwOnException = true)
        {
            try
            {
                return _encryptionService.Decrypt(secret);
            }
            catch (Exception e)
            {
                if (throwOnException)
                {
                    throw new ArgumentException("Couldn't decrypt application secret. Please enter it again.", e);
                }
            }
            return null;
        }

        public void Validate(T settings)
        {
            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                throw new ArgumentException("API key is required");
            }

            if (string.IsNullOrEmpty(settings.ApplicationKey))
            {
                throw new ArgumentException("Application key is required");
            }

            if (string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                throw new ArgumentException("Application secret is required");
            }

            if (string.IsNullOrEmpty(settings.DataCenter))
            {
                throw new ArgumentException("DataCenter is required");
            }

            if (string.IsNullOrEmpty(settings.Language))
            {
                throw new ArgumentException("Language is required");
            }

            if (settings.Language == Constants.Languages.Auto && string.IsNullOrEmpty(settings.LanguageFallback))
            {
                throw new ArgumentException("Language fallback is required");
            }

            if (settings.SessionProvider == Enums.GigyaSessionProvider.Gigya)
            {
                if (settings.GigyaSessionMode == Enums.GigyaSessionMode.Sliding && settings.SessionTimeout <= 0)
                {
                    throw new ArgumentException("Session duration must be greater than 0 if sliding session mode enabled");
                }

                if (settings.GigyaSessionMode == Enums.GigyaSessionMode.Fixed && settings.SessionTimeout <= 0)
                {
                    throw new ArgumentException("Session duration must be greater than 0 if fixed session mode enabled");
                }
            }

            if (!string.IsNullOrEmpty(settings.GlobalParameters))
            {
                try
                {
                    JsonConvert.DeserializeObject<dynamic>(settings.GlobalParameters);
                }
                catch
                {
                    throw new ArgumentException("Couldn't deserialize global parameters. Check it's valid JSON.");
                }
            }
        }
    }
}
