using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Connector.Common
{
    public static class UrlUtils
    {
        public static string AddQueryStringParam(string url, string param)
        {
            var queryStringSeparator = url.Contains('?') ? "&" : "?";
            return string.Concat(url, queryStringSeparator, param);
        }
    }
}