using Gigya.Module.Core.Connector.Helpers;
using Gigya.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.XPath;
using SC = Sitecore;
using Core = Gigya.Module.Core;
using Sitecore.Data.Items;
using Sitecore.Gigya.Extensions.Extensions;
using Sitecore.Data.Fields;
using System.Web.Mvc;
using Sitecore.Gigya.Module.Encryption;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaSettingsHelper : Core.Connector.Helpers.GigyaSettingsHelper, IGigyaSettingsHelper
    {
        private static string _cmsVersion { get; set; }
        private static string _cmsMajorVersion { get; set; }

        public GigyaSettingsHelper() : base(SitecoreEncryptionService.Instance)
        {
        }

        public override string CmsName
        {
            get
            {
                return "Sitecore";
            }
        }

        public string CmsMajorVersion { get { return _cmsMajorVersion; } }

        public override string CmsVersion
        {
            get
            {
                return _cmsVersion;
            }
        }

        public override string ModuleVersion
        {
            get
            {
                return Constants.ModuleVersion;
            }
        }        

        private static void LoadCmsVersion()
        {
            var versionInfo = File.ReadAllText(HostingEnvironment.MapPath("~/sitecore/shell/sitecore.version.xml"));
            XDocument doc = XDocument.Parse(versionInfo);
            _cmsMajorVersion = doc.XPathSelectElement("//information/version/major")?.Value;
            var minorVersion = doc.XPathSelectElement("//information/version/minor")?.Value;
            var revision = doc.XPathSelectElement("//information/version/revision")?.Value;

            _cmsVersion = string.Format("{0}.{1}.{2}", _cmsMajorVersion, minorVersion, revision);
        }

        static GigyaSettingsHelper()
        {
            LoadCmsVersion();
        }

        public override IGigyaModuleSettings GetForCurrentSite(bool decrypt = false)
        {
            var model = Get(Context.Site.Name, decrypt);

            // if we are using global settings we still want to tell the client to use the current site name
            model.Id = Context.Site.Name;
            return model;
        }

        protected override List<IGigyaModuleSettings> GetForSiteAndDefault(object id)
        {
            var result = new List<IGigyaModuleSettings>();

            using (SecurityModel.SecurityDisabler disabler = new SecurityModel.SecurityDisabler())
            {
                var globalSettings = Context.Database.GetItem(Constants.Paths.GlobalSettings);
                if (globalSettings != null)
                {
                    result.Add(Map(globalSettings, Constants.GlobalSettingsId));
                }

                var currentSiteSettingsPath = Context.Site.GigyaSiteSettings() ?? string.Concat(Context.Site.StartPath, "/", Constants.Paths.SiteSettingsSuffix);
                var currentSiteSettings = Context.Database.GetItem(currentSiteSettingsPath);
                if (currentSiteSettings != null)
                {
                    result.Add(Map(currentSiteSettings, Context.Site.Name));
                }
            }

            return result;
        }

        private IGigyaModuleSettings Map(Item settings, string id)
        {
            var mapped = new GigyaModuleSettings
            {
                Id = id,
                ApiKey = settings.Fields[Constants.Fields.ApiKey].Value.Trim(),
                ApplicationKey = settings.Fields[Constants.Fields.ApplicationKey].Value.Trim(),
                ApplicationSecret = settings.Fields[Constants.Fields.ApplicationSecret].Value.Trim(),
                Language = settings.Fields[Constants.Fields.Language].Value.Trim(),
                //LanguageFallback = settings.Fields[Constants.Fields.LanguageFallback,
                DebugMode = ((CheckboxField)settings.Fields[Constants.Fields.DebugMode]).Checked,
                DataCenter = StringHelper.FirstNotNullOrEmpty(settings.Fields[Constants.Fields.DataCenter].Value, Constants.DefaultSettings.DataCenter),
                EnableRaas = ((CheckboxField)settings.Fields[Constants.Fields.EnableRaaS]).Checked,
                RedirectUrl = settings.Fields[Constants.Fields.RedirectUrl].Value,
                LogoutUrl = settings.Fields[Constants.Fields.LogoutUrl].Value,
                //MappingFields = settings.Fields[Constants.Fields.MembershipMappingFields].Value,
                GlobalParameters = settings.Fields[Constants.Fields.GlobalParameters].Value,
                SessionTimeout = int.Parse(StringHelper.FirstNotNullOrEmpty(settings.Fields[Constants.Fields.GigyaSessionDuration].Value, Constants.DefaultSettings.SessionTimeout)),
                SessionProvider = Core.Connector.Enums.GigyaSessionProvider.Gigya,
                GigyaSessionMode = Core.Connector.Enums.GigyaSessionMode.Sliding
            };

            if (!string.IsNullOrEmpty(mapped.ApplicationSecret) && mapped.ApplicationSecret.StartsWith(Constants.EncryptionPrefix))
            {
                mapped.ApplicationSecret = mapped.ApplicationSecret.Substring(Constants.EncryptionPrefix.Length);
            }

            Core.Connector.Enums.GigyaSessionMode sessionMode;
            if (Enum.TryParse(settings.Fields[Constants.Fields.GigyaSessionType].Value, out sessionMode))
            {
                mapped.GigyaSessionMode = sessionMode;
            }

            return mapped;
        }

        protected override string Language(IGigyaModuleSettings settings)
        {
            var languageHelper = new GigyaLanguageHelper();
            return languageHelper.Language(settings, SC.Context.Language.CultureInfo);
        }

        protected override string ClientScriptPath(IGigyaModuleSettings settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-cms.js" : "gigya-cms.min.js";
            return string.Concat("~/scripts/gigya/", scriptName);
        }

        public override void Delete(object id)
        {
            throw new NotImplementedException();
        }

        protected override IGigyaModuleSettings EmptySettings(object id)
        {
            return new GigyaModuleSettings { Id = id, DebugMode = true, EnableRaas = true };
        }
    }
}
