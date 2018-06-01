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
            if (current == null || current.ID.IsNull)
            {
                return null;
            }

            if (current.TemplateID == Constants.Templates.GigyaSettings)
            {
                return current;
            }

            return GetSettingsParent(current.Parent);
        }
    }
}