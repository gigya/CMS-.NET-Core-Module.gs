using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gigya.Module.Data;
using Gigya.Module.Connector.Encryption;
using System.Reflection;
using System.Diagnostics;
using Telerik.Sitefinity.Services;
using System.Globalization;
using Gigya.Module.Mvc.Models;
using Newtonsoft.Json;
using System.Dynamic;
using Gigya.Module.Connector.Common;

using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using System.Web.Mvc;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaSettingsHelper
    {
        public static string SitefinityVersion { get; private set; }

        private static void LoadSitefinityAssemblyVersion()
        {
            var assembly = Assembly.Load("Telerik.Sitefinity");
            SitefinityVersion = string.Concat("Sitefinity.", FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);
        }

        static GigyaSettingsHelper()
        {
            LoadSitefinityAssemblyVersion();
        }

        /// <summary>
        /// Creates a view model for use in a view.
        /// </summary>
        /// <param name="settings">The settings for the current site.</param>
        /// <param name="urlHelper">UrlHelper for the current request.</param>
        public static GigyaSettingsViewModel ViewModel(GigyaModuleSettings settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-sitefinity.js" : "gigya-sitefinity.min.js";
            var scriptPath = FileHelper.GetPath("~/Mvc/Scripts/" + scriptName, urlHelper.WidgetContent("Mvc/Scripts/" + scriptName));

            var model = new GigyaSettingsViewModel
            {
                ApiKey = settings.ApiKey,
                DebugMode = settings.DebugMode,
                GigyaScriptPath = UrlUtils.AddQueryStringParam(scriptPath, "v=" + ModuleClass.Version)
            };
            
            model.Settings = !string.IsNullOrEmpty(settings.GlobalParameters) ? JsonConvert.DeserializeObject<dynamic>(settings.GlobalParameters) : new ExpandoObject();
            model.Settings.lang = GigyaLanguageHelper.Language(settings);
            model.Settings.sessionExpiration = settings.SessionTimeout;
            model.SettingsJson = JsonConvert.SerializeObject(model.Settings);
            return model;
        }

        /// <summary>
        /// Gets the Gigya module settings for the current site.
        /// </summary>
        /// <param name="decrypt">Whether to decrypt the application secret.</param>
        public static GigyaModuleSettings GetForCurrentSite(bool decrypt = false)
        {
            var siteId = Guid.Empty;
            if (SystemManager.CurrentContext.IsMultisiteMode)
            {
                siteId = SystemManager.CurrentContext.CurrentSite.Id;
            }

            return Get(siteId, decrypt);
        }

        /// <summary>
        /// Deletes the Gigya module settings for the site with id of <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">Id of the site whose settings will be deleted.</param>
        internal static void Delete(Guid siteId)
        {
            using (var context = GigyaContext.Get())
            {
                var setting = context.Settings.FirstOrDefault(i => i.SiteId == siteId);
                if (setting != null)
                {
                    context.Delete(setting);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets Gigya Module settings for <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The Id of the site.</param>
        /// <param name="decrypt">Whether to decrypt the application secret.</param>
        public static GigyaModuleSettings Get(Guid siteId, bool decrypt = false)
        {
            GigyaModuleSettings settings = null;

            var context = GigyaContext.Get();
            var siteSettingsAndGlobal = context.Settings.Where(i => i.SiteId == siteId || i.SiteId == Guid.Empty).ToList();
            settings = siteSettingsAndGlobal.FirstOrDefault(i => i.SiteId == siteId) ?? siteSettingsAndGlobal.FirstOrDefault() ?? new GigyaModuleSettings { SiteId = siteId, DebugMode = true };

            // decrypt application secret
            if (decrypt && !string.IsNullOrEmpty(settings.ApplicationSecret))
            {
                settings.ApplicationSecret = Encryptor.Decrypt(settings.ApplicationSecret);
            }

            return settings;
        }
    }
}
