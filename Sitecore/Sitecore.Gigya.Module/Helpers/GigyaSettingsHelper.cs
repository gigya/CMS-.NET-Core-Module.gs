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
using Sitecore.Data.Fields;
using System.Web.Mvc;
using Sitecore.Gigya.Module.Encryption;
using Sitecore.Data;
using Sitecore.Gigya.Module.Models;
using Gigya.Module.Core.Connector.Models;
using System.Web.Security;
using System.Web;

using A = Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Gigya.Module.Core.Connector.Extensions;
using Sitecore.Gigya.Module.Extensions;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaSettingsHelper : Core.Connector.Helpers.GigyaSettingsHelper<SitecoreGigyaModuleSettings>
    {
        protected readonly IGigyaUserProfileHelper _userProfileHelper;
        private static string _cmsVersion { get; set; }
        private static string _cmsMajorVersion { get; set; }

        public GigyaSettingsHelper() : this(new GigyaUserProfileHelper())
        {
        }

        public GigyaSettingsHelper(IGigyaUserProfileHelper gigyaUserProfileHelper) : base(SitecoreEncryptionService.Instance)
        {
            LoadCmsVersion();
            _userProfileHelper = gigyaUserProfileHelper;
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
        
        protected virtual string ReadVersionFile()
        {
            var versionInfo = File.ReadAllText(HostingEnvironment.MapPath("~/sitecore/shell/sitecore.version.xml"));
            return versionInfo;
        }

        private void LoadCmsVersion()
        {
            if (!string.IsNullOrEmpty(_cmsVersion))
            {
                return;
            }

            var versionInfo = ReadVersionFile();
            XDocument doc = XDocument.Parse(versionInfo);
            _cmsMajorVersion = doc.XPathSelectElement("//information/version/major")?.Value;
            var minorVersion = doc.XPathSelectElement("//information/version/minor")?.Value;
            var revision = doc.XPathSelectElement("//information/version/revision")?.Value;

            _cmsVersion = string.Format("{0}.{1}.{2}", _cmsMajorVersion, minorVersion, revision);
        }

        public override SitecoreGigyaModuleSettings GetForCurrentSite(bool decrypt = false)
        {
            var model = Get(Context.Site.Name, decrypt);

            // if we are using global settings we still want to tell the client to use the current site name
            model.Id = Context.Site.Name;
            return model;
        }

        protected override List<SitecoreGigyaModuleSettings> GetForSiteAndDefault(object id)
        {
            var result = new List<SitecoreGigyaModuleSettings>();
            if (Context.Database == null)
            {
                return result;
            }

            using (SecurityModel.SecurityDisabler disabler = new SecurityModel.SecurityDisabler())
            {
                var globalSettings = GetGlobalSettings();
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

        private Item GetGlobalSettings()
        {
            var globalSettings = Context.Database.GetItem(Constants.Ids.GlobalSettings);
            if (globalSettings != null)
            {
                return globalSettings;
            }

            // try and find global settings based on path and template
            var moduleFolder = Context.Database.GetItem(Constants.Paths.ModulePath);
            globalSettings = moduleFolder.Children.FirstOrDefault(i => i.TemplateID == Constants.Templates.GigyaSettings);
            return globalSettings;
        }

        public SitecoreGigyaModuleSettings Map(Item settings, string id)
        {
            return Map(settings, id, Context.Database);
        }

        public SitecoreGigyaModuleSettings Map(Item settings, string id, Database database)
        {
            var mapped = new SitecoreGigyaModuleSettings
            {
                Id = id,
                ApiKey = settings.Fields[Constants.Fields.ApiKey].Value.Trim(),
                ApplicationKey = settings.Fields[Constants.Fields.ApplicationKey].Value.Trim(),
                ApplicationSecret = settings.Fields[Constants.Fields.ApplicationSecret].Value.Trim(),
                Language = settings.Fields[Constants.Fields.Language].Value.Trim(),
                LanguageFallback = Constants.DefaultSettings.LanguageFallback,
                DebugMode = ((CheckboxField)settings.Fields[Constants.Fields.DebugMode]).Checked,
                DataCenter = Constants.DefaultSettings.DataCenter,
                EnableRaas = ((CheckboxField)settings.Fields[Constants.Fields.EnableRaaS]).Checked,
                EnableMembershipSync = ((CheckboxField)settings.Fields[Constants.Fields.EnableMembershipProviderSync]).Checked,
                EnableXdb = ((CheckboxField)settings.Fields[Constants.Fields.EnableXdbSync]).Checked,
                RedirectUrl = ((LinkField)settings.Fields[Constants.Fields.RedirectUrl]).GetFriendlyUrl().ToAbsoluteUrl(),
                LogoutUrl = ((LinkField)settings.Fields[Constants.Fields.LogoutUrl]).GetFriendlyUrl().ToAbsoluteUrl(),
                GlobalParameters = settings.Fields[Constants.Fields.GlobalParameters].Value.Trim(),
                SessionTimeout = System.Convert.ToInt32(FormsAuthentication.Timeout.TotalSeconds),
                SessionProvider = Core.Connector.Enums.GigyaSessionProvider.Gigya,
                GigyaSessionMode = Core.Connector.Enums.GigyaSessionMode.Sliding,
                ProfileId = _userProfileHelper.GetSelectedProfile(settings)?.ID?.ToString()
            };

            MapMappingFields(settings, mapped);
            ExtractDataCenter(settings, mapped, database);

            if (!string.IsNullOrEmpty(mapped.ApplicationSecret) && mapped.ApplicationSecret.StartsWith(Constants.EncryptionPrefix))
            {
                mapped.ApplicationSecret = mapped.ApplicationSecret.Substring(Constants.EncryptionPrefix.Length);
            }

            MapSessionMode(settings, mapped);

            return mapped;
        }

        private void MapMappingFields(Item settings, SitecoreGigyaModuleSettings mapped)
        {
            var mappingFieldSourceItem = settings;
            var parentSettingsId = ((MultilistField)settings.Fields[Constants.Fields.Parent]).Items.FirstOrDefault();
            if (!string.IsNullOrEmpty(parentSettingsId))
            {
                var parentSettings = settings.Database.GetItem(new ID(parentSettingsId));
                if (parentSettings != null)
                {
                    mappingFieldSourceItem = parentSettings;
                }
            }
            mapped.MappedMappingFields = ExtractMappingFields(mappingFieldSourceItem, MappingFieldType.Membership);
            mapped.MappedXdbMappingFields = ExtractXdbMappingFields(mappingFieldSourceItem);
        }

        private static void MapSessionMode(Item settings, SitecoreGigyaModuleSettings mapped)
        {
            if (((CheckboxField)settings.Fields[Constants.Fields.SessionCookieMode]).Checked)
            {
                mapped.GigyaSessionMode = Core.Connector.Enums.GigyaSessionMode.Session;
            }
            else if (FormsAuthentication.SlidingExpiration)
            {
                mapped.GigyaSessionMode = Core.Connector.Enums.GigyaSessionMode.Sliding;
            }
            else
            {
                mapped.GigyaSessionMode = Core.Connector.Enums.GigyaSessionMode.Fixed;
            }
        }

        private A.MappingFieldGroup ExtractXdbMappingFields(Item settings)
        {
            var folder = settings.Children.FirstOrDefault(i => i.TemplateID == Constants.Templates.xDbMappingFieldFolder);
            if (folder == null)
            {
                return new A.MappingFieldGroup();
            }

            var mapping = Mapper.MapMappingFieldGroup(folder);
            return mapping;
        }

        private List<MappingField> ExtractMappingFields(Item settings, MappingFieldType fieldType)
        {
            var fieldTypeString = fieldType.ToString();
            var folder = settings.Children.FirstOrDefault(i => i.TemplateID == Constants.Templates.MappingFieldFolder && i.Fields[Constants.Fields.MappingFieldFolder.Type].Value == fieldTypeString);
            if (folder == null)
            {
                return new List<MappingField>();
            }

            var fields = folder.Children.Where(i => i.TemplateID == Constants.Templates.MappingField).Select(Mapper.Map).ToList();
            return fields;
        }

        private void ExtractDataCenter(Item settings, GigyaModuleSettings mapped, Database database)
        {
            ID dataCenterItemId;
            if (!ID.TryParse(settings.Fields[Constants.Fields.DataCenter].Value, out dataCenterItemId))
            {
                return;
            }

            var dataCenterItem = database.GetItem(dataCenterItemId);
            if (dataCenterItem == null)
            {
                return;
            }

            var dataCenterValue = dataCenterItem.Fields[Constants.Fields.DataCenter].Value;
            if (!string.IsNullOrEmpty(dataCenterValue))
            {
                mapped.DataCenter = dataCenterValue;
            }
        }

        public override string TryDecryptApplicationSecret(string secret, bool throwOnException = true)
        {
            if (string.IsNullOrEmpty(secret))
            {
                return null;
            }

            if (secret.StartsWith(Constants.EncryptionPrefix))
            {
                secret = secret.Substring(Constants.EncryptionPrefix.Length);
            }

            return base.TryDecryptApplicationSecret(secret, throwOnException);
        }

        protected override string Language(SitecoreGigyaModuleSettings settings)
        {
            var languageHelper = new GigyaLanguageHelper();
            return languageHelper.Language(settings, SC.Context.Language.CultureInfo);
        }

        protected override string ClientScriptPath(SitecoreGigyaModuleSettings settings, UrlHelper urlHelper)
        {
            var scriptName = settings.DebugMode ? "gigya-cms.js" : "gigya-cms.min.js";
            return string.Concat("~/scripts/gigya/", scriptName);
        }

        public override void Delete(object id)
        {
            throw new NotImplementedException();
        }

        protected override SitecoreGigyaModuleSettings EmptySettings(object id)
        {
            return new SitecoreGigyaModuleSettings { Id = id, DebugMode = true, EnableRaas = true };
        }

        public SessionValidationModel IsSessionSettingsValid(SitecoreGigyaModuleSettings settings)
        {
            var response = new SessionValidationModel();

            var context = HttpContext.Current;
            if (context == null)
            {
                // happens when installing the module
                response.IsValid = true;
                return response;
            }
            
            var formsTimeoutSeconds = FormsAuthentication.Timeout.TotalSeconds;

            switch (settings.GigyaSessionMode)
            {
                case Core.Connector.Enums.GigyaSessionMode.Fixed:
                case Core.Connector.Enums.GigyaSessionMode.Sliding:
                    if (settings.SessionTimeout != formsTimeoutSeconds)
                    {
                        response.Message = string.Format("Gigya session timeout of {0} seconds doesn't match Sitecore's timeout of {1} seconds set in web.config.", settings.SessionTimeout, formsTimeoutSeconds);
                        return response;
                    }
                    break;
            }

            response.IsValid = true;
            return response;
        }
    }
}
