using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module
{
    public class SitefinityUtils
    {
        public static string GetPageUrl(Guid? pageId)
        {
            if (!pageId.HasValue)
            {
                return null;
            }

            var node = SiteMap.Provider.FindSiteMapNodeFromKey(pageId.Value.ToString());
            if (node != null)
            {
                return VirtualPathUtility.ToAbsolute(node.Url);
            }

            return null;
        }
    }
}