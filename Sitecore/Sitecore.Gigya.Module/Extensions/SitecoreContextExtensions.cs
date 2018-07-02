using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Module.Extensions
{
    public static class SitecoreContextExtensions
    {
        /// <summary>
        /// Return the path to the site's Gigya settings.
        /// </summary>
        /// <returns></returns>
        public static string GigyaSiteSettings(this SiteContext site)
        {
            var path = site.Properties["gigyaSettings"];
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return path;
        }
    }
}