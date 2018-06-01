using Gigya.Module.Core.Connector.Helpers;
using Newtonsoft.Json;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Logging;
using Sitecore.Gigya.Module.Models;
using Gigya.Module.Core.Data;
using Sitecore.Gigya.Extensions.Attributes;
using Gigya.Module.Core.Mvc.Attributes;

namespace Sitecore.Gigya.Module.Controllers
{
    [SitecoreUserAuthorize]
    public class ContentEditorController : Controller
    {
        private readonly ISitecoreContentHelper _sitecoreContentHelper;

        public ContentEditorController() : this(new SitecoreContentHelper())
        {
        }

        public ContentEditorController(ISitecoreContentHelper sitecoreContentHelper)
        {
            _sitecoreContentHelper = sitecoreContentHelper;
        }

        [NoCache]
        public ActionResult Results(string id, string query)
        {
            if (Context.ContentDatabase == null)
            {
                return new HttpUnauthorizedResult();
            }

            var item = Context.ContentDatabase.GetItem(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var settingsItem = _sitecoreContentHelper.GetSettingsParent(item);

            var settingsHelper = new Helpers.GigyaSettingsHelper();
            var settings = settingsHelper.Map(settingsItem, Context.Site.Name, Context.ContentDatabase);
            settingsHelper.DecryptApplicationSecret(ref settings);

            var logger = LoggerFactory.Instance();
            var gigyaAccountSchemaHelper = new GigyaAccountSchemaHelper<SitecoreGigyaModuleSettings>(new GigyaApiHelper<SitecoreGigyaModuleSettings>(settingsHelper, logger), settings);
            var schema = gigyaAccountSchemaHelper.GetAccountSchema();

            var results = new AutocompleteResult();
            var properties = schema.Properties;
            if (!string.IsNullOrEmpty(query))
            {
                // apply filter
                properties = properties.Where(i => i.Name.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
            }

            results.Suggestions = properties.Select(Mapper.Map).ToList();

            var data = JsonConvert.SerializeObject(results);
            return Content(data, "application/json");
        }
    }
}