using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gigya.Module.Data;
using System.Reflection;
using System.Diagnostics;
using Telerik.Sitefinity.Services;
using System.Globalization;

using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using System.Web.Mvc;
using Gigya.Module.Core.Data;
using Gigya.Module.Core.Connector.Helpers;
using System.Web;
using Gigya.Module.Core.Mvc.Models;
using Gigya.Module.Core.Connector.Common;

namespace Gigya.Module.Connector.Helpers
{
    public class GigyaSettingsHelper : Gigya.Module.Core.Connector.Helpers.GigyaSettingsHelper
    {
        public override string CmsName
        {
            get
            {
                return "Sitefinity";
            }
        }

        public override string CmsVersion
        {
            get
            {
                return _sitefinityVersion;
            }
        }

        public override string ModuleVersion
        {
            get
            {
                return ModuleClass.Version;
            }
        }

        private static string _sitefinityVersion { get; set; }

        private static void LoadSitefinityAssemblyVersion()
        {
            var assembly = Assembly.Load("Telerik.Sitefinity");
            _sitefinityVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }

        static GigyaSettingsHelper()
        {
            LoadSitefinityAssemblyVersion();
        }

        public override GigyaModuleSettings GetForCurrentSite(bool decrypt = false)
        {
            var siteId = Guid.Empty;
            if (SystemManager.CurrentContext.IsMultisiteMode)
            {
                siteId = SystemManager.CurrentContext.CurrentSite.Id;
            }

            var model = Get(siteId, decrypt);

            // if we are using global settings we still want to tell the client to use the current homepage id
            model.Id = siteId;
            return model;
        }

        protected override List<GigyaModuleSettings> GetForSiteAndDefault(object id)
        {
            var idList = id as string[];
            if (idList != null)
            {
                id = idList[0];
            }

            var siteId = Guid.Parse(id.ToString());
            using (var context = GigyaContext.Get())
            {
                return context.Settings.Where(i => i.SiteId == siteId || i.SiteId == Guid.Empty).Select(Map).ToList();
            }
        }

        private GigyaModuleSettings Map(GigyaSitefinityModuleSettings settings)
        {
            return new GigyaModuleSettings
            {
                Id = settings.SiteId,
                ApiKey = settings.ApiKey,
                ApplicationKey = settings.ApplicationKey,
                ApplicationSecret = settings.ApplicationSecret,
                Language = settings.Language,
                LanguageFallback = settings.LanguageFallback,
                DebugMode = settings.DebugMode,
                DataCenter = settings.DataCenter,
                EnableRaas = settings.EnableRaas,
                RedirectUrl = settings.RedirectUrl,
                LogoutUrl = settings.LogoutUrl,
                MappingFields = settings.MappingFields,
                GlobalParameters = settings.GlobalParameters,
                SessionTimeout = settings.SessionTimeout,
                SessionProvider = settings.SessionProvider,
                GigyaSessionMode = settings.GigyaSessionMode
            };
        }

        protected override string Language(GigyaModuleSettings settings)
        {
            var languageHelper = new GigyaLanguageHelper();

            var languageKey = CultureInfo.CurrentUICulture.Name.ToLowerInvariant();
            var currentSite = SystemManager.CurrentContext.CurrentSite;
            var cultures = currentSite.PublicContentCultures;
            if (cultures != null && cultures.Length == 1 && !string.IsNullOrEmpty(currentSite.DefaultCulture))
            {
                languageKey = currentSite.DefaultCulture.ToLowerInvariant();
            }

            var culture = new CultureInfo(languageKey);
            return languageHelper.Language(settings, culture);
        }

        protected override string ClientScriptPath(GigyaModuleSettings settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-sitefinity.js" : "gigya-sitefinity.min.js";
            var scriptPath = FileHelper.GetPath("~/Mvc/Scripts/" + scriptName, urlHelper.WidgetContent("Mvc/Scripts/" + scriptName, ModuleClass.AssemblyName));

            return scriptPath;
        }

        /// <summary>
        /// Deletes the Gigya module settings for the site with id of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the site whose settings will be deleted.</param>
        public override void Delete(object id)
        {
            var siteId = Guid.Parse(id.ToString());
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

        protected override GigyaModuleSettings EmptySettings(object id)
        {
            return new GigyaModuleSettings { Id = id, DebugMode = true, EnableRaas = true };
        }
    }
}
