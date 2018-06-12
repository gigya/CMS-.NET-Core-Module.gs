using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gigya.Module.Core.Connector.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a relative URL into a fully qualified URL.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string ToAbsoluteUrl(this string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl) || relativeUrl.StartsWith("http://") || relativeUrl.StartsWith("https://") || relativeUrl.StartsWith("//"))
            {
                return relativeUrl;
            }

            var context = HttpContext.Current;
            if (context == null)
            {
                return relativeUrl;
            }

            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Insert(0, "/");
            }

            var url = context.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : string.Empty;
            return string.Concat(url.Scheme, "://", url.Host, port, relativeUrl);
        }
    }
}
