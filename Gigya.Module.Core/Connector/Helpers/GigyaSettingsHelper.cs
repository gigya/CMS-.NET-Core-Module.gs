﻿using System;
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
    public abstract class GigyaSettingsHelper
    {
        private IPathUtilities _pathUtilities = new PathUtilities();
        public IPathUtilities PathUtilities
        {
            set { _pathUtilities = value; }
        }

        private const string _cacheKeyBase = "GigyaSettingsHelper-6A0A6E65-F65B-4197-8D4E-2BDCD5680EAF";
        protected abstract string Language(IGigyaModuleSettings settings);
        public abstract string CmsName { get; }
        public abstract string CmsVersion { get; }
        public abstract string ModuleVersion { get; }

        /// <summary>
        /// Gets the Gigya module settings for the current site.
        /// </summary>
        /// <param name="decrypt">Whether to decrypt the application secret.</param>
        public abstract IGigyaModuleSettings GetForCurrentSite(bool decrypt = false);

        /// <summary>
        /// Deletes the Gigya module settings for the site with id of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the site whose settings will be deleted.</param>
        public abstract void Delete(object id);

        protected abstract List<IGigyaModuleSettings> GetForSiteAndDefault(object id);

        protected abstract IGigyaModuleSettings EmptySettings(object id);

        protected virtual string ClientScriptPath(IGigyaModuleSettings settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-cms.js" : "gigya-cms.min.js";
            return string.Concat("~/scripts/", scriptName);
        }

        /// <summary>
        /// Creates a view model for use in a view.
        /// </summary>
        /// <param name="settings">The settings for the current site.</param>
        /// <param name="urlHelper">UrlHelper for the current request.</param>
        public virtual GigyaSettingsViewModel ViewModel(IGigyaModuleSettings settings, UrlHelper urlHelper, CurrentIdentity currentIdentity)
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
                DataCenter = settings.DataCenter
            };
            
            model.Settings = !string.IsNullOrEmpty(settings.GlobalParameters) ? JsonConvert.DeserializeObject<ExpandoObject>(settings.GlobalParameters) : new ExpandoObject();

            var settingsProperties = (IDictionary<string, object>)model.Settings;
            if (!settingsProperties.ContainsKey("lang"))
            {
                model.Settings.lang = Language(settings);
            }
            if (!settingsProperties.ContainsKey("sessionExpiration"))
            {
                model.Settings.sessionExpiration = settings.SessionTimeout;
            }

            model.SettingsJson = JsonConvert.SerializeObject(model.Settings);            
            return model;
        }

        public int SessionExpiration(IGigyaModuleSettings settings)
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
        public virtual IGigyaModuleSettings Get(object id, bool decrypt = false)
        {
            var cacheKey = string.Concat(_cacheKeyBase, id, decrypt);
            IGigyaModuleSettings settings = null;

            var context = HttpContext.Current;
            if (context != null)
            {
                settings = context.Items[cacheKey] as IGigyaModuleSettings;
                if (settings != null)
                {
                    return settings;
                }
            }
            
            var siteSettingsAndGlobal = GetForSiteAndDefault(id);
            settings = siteSettingsAndGlobal.OrderByDescending(i => i.Id).FirstOrDefault() ?? EmptySettings(id);

            // decrypt application secret
            if (decrypt && !string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                settings.ApplicationSecret = Encryptor.Decrypt(settings.ApplicationSecret);
            }

            settings.MappedMappingFields = !string.IsNullOrEmpty(settings.MappingFields) 
                ? JsonConvert.DeserializeObject<List<MappingField>>(settings.MappingFields) : new List<MappingField>();

            if (context != null)
            {
                // cache for the rest of this request
                context.Items[cacheKey] = settings;
            }

            return settings;
        }

        /// <summary>
        /// Decrypts an application secret if required.
        /// </summary>
        public virtual void DecryptApplicationSecret(ref IGigyaModuleSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                settings.ApplicationSecret = Encryptor.Decrypt(settings.ApplicationSecret);
            }
        }

        public void Validate(IGigyaModuleSettings settings)
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
