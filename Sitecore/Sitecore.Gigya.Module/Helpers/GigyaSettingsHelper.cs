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
using SC = Sitecore;
using Core = Gigya.Module.Core;

namespace Sitecore.Gigya.Module.Helpers
{
    public class GigyaSettingsHelper : Core.Connector.Helpers.GigyaSettingsHelper
    {
        private static string _cmsVersion { get; set; }
        private static string _cmsMajorVersion { get; set; }

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
            _cmsMajorVersion = doc.Element("/information/major").Value;
            var minorVersion = doc.Element("/information/minor").Value;
            var revision = doc.Element("/information/revision").Value;

            _cmsVersion = string.Format("{0}.{1}.{2}", _cmsMajorVersion, minorVersion, revision);
        }

        static GigyaSettingsHelper()
        {
            LoadCmsVersion();
        }

        public override IGigyaModuleSettings GetForCurrentSite(bool decrypt = false)
        {
            throw new NotImplementedException();

            //var siteId = Guid.Empty;
            //if (SystemManager.CurrentContext.IsMultisiteMode)
            //{
            //    siteId = SystemManager.CurrentContext.CurrentSite.Id;
            //}

            //var model = Get(siteId, decrypt);

            //// if we are using global settings we still want to tell the client to use the current homepage id
            //model.Id = siteId;
            //return model;
        }

        protected override List<IGigyaModuleSettings> GetForSiteAndDefault(object id)
        {
            throw new NotImplementedException();
            //var idList = id as string[];
            //if (idList != null)
            //{
            //    id = idList[0];
            //}

            //var siteId = Guid.Parse(id.ToString());
            //using (var context = GigyaContext.Get())
            //{
            //    return context.Settings.Where(i => i.SiteId == siteId || i.SiteId == Guid.Empty).Select(Map).ToList();
            //}
        }

        //private IGigyaModuleSettings Map(GigyaModuleSettings settings)
        //{
        //    return new GigyaModuleSettings
        //    {
        //        Id = settings.SiteId,
        //        ApiKey = settings.ApiKey,
        //        ApplicationKey = settings.ApplicationKey,
        //        ApplicationSecret = settings.ApplicationSecret,
        //        Language = settings.Language,
        //        LanguageFallback = settings.LanguageFallback,
        //        DebugMode = settings.DebugMode,
        //        DataCenter = settings.DataCenter,
        //        EnableRaas = settings.EnableRaas,
        //        RedirectUrl = settings.RedirectUrl,
        //        LogoutUrl = settings.LogoutUrl,
        //        MappingFields = settings.MappingFields,
        //        GlobalParameters = settings.GlobalParameters,
        //        SessionTimeout = settings.SessionTimeout,
        //        SessionProvider = settings.SessionProvider,
        //        GigyaSessionMode = settings.GigyaSessionMode
        //    };
        //}

        protected override string Language(IGigyaModuleSettings settings)
        {
            var languageHelper = new GigyaLanguageHelper();
            return languageHelper.Language(settings, SC.Context.Language.CultureInfo);
        }

        //protected override string ClientScriptPath(IGigyaModuleSettings settings, UrlHelper urlHelper)
        //{
        //    throw new NotImplementedException();
        //    //var scriptName = settings.DebugMode ? "gigya-sitefinity.js" : "gigya-sitefinity.min.js";
        //    //var scriptPath = FileHelper.GetPath("~/Mvc/Scripts/" + scriptName, urlHelper.WidgetContent("Mvc/Scripts/" + scriptName, ModuleClass.AssemblyName));

        //    //return scriptPath;
        //}

        /// <summary>
        /// Deletes the Gigya module settings for the site with id of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the site whose settings will be deleted.</param>
        public override void Delete(object id)
        {
            throw new NotImplementedException();
            //var siteId = Guid.Parse(id.ToString());
            //using (var context = GigyaContext.Get())
            //{
            //    var setting = context.Settings.FirstOrDefault(i => i.SiteId == siteId);
            //    if (setting != null)
            //    {
            //        context.Delete(setting);
            //        context.SaveChanges();
            //    }
            //}
        }

        protected override IGigyaModuleSettings EmptySettings(object id)
        {
            return new GigyaModuleSettings { Id = id, DebugMode = true, EnableRaas = true };
        }
    }
}
