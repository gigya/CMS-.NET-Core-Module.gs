using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Helpers
{
    public class SitecoreContentHelper : ISitecoreContentHelper
    {
        public Item GetSettingsParent(Item current)
        {
            return GetClosestTemplate(current, Constants.Templates.GigyaSettings);
        }

        public Item GetFacetFolderParent(Item current)
        {
            return GetClosestTemplate(current, Constants.Templates.SitecoreXdbFacetFolder);
        }

        protected virtual Item GetClosestTemplate(Item current, Data.ID templateId)
        {
            if (current == null || current.ID.IsNull)
            {
                return null;
            }

            if (current.TemplateID == templateId)
            {
                return current;
            }

            return GetClosestTemplate(current.Parent, templateId);
        }
    }
}