using Sitecore.Data;
using Sitecore.Xml.Xsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Sitecore.Gigya.Module.Fields
{
    public class ExtendedLinkUrl : LinkUrl
    {
        public string GetUrl(string linkFieldXml, Database database)
        {
            if (string.IsNullOrEmpty(linkFieldXml) || database == null)
            {
                return null;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(linkFieldXml);
            var linkElement = xmlDocument.DocumentElement;

            if (linkElement == null)
            {
                return null;
            }

            string linkType = linkElement.GetAttribute("linktype");
            string url = linkElement.GetAttribute("url");
            string id = linkElement.GetAttribute("id");
            string anchor = linkElement.GetAttribute("anchor");
            string queryString = linkElement.GetAttribute("querystring");

            if (!string.IsNullOrEmpty(anchor))
            {
                anchor = "#" + anchor;
            }

            switch (linkType)
            {
                case "anchor":
                    return anchor;
                case "external":
                    return GetExternalUrl(url);
                case "internal":
                    return GetInternalUrl(database, url, id, anchor, queryString);
                case "javascript":
                    return GetJavaScriptUrl(url);
                case "mailto":
                    return GetMailToLink(url);
                case "media":
                    return GetMediaUrl(database, id);
                default:
                    return string.Empty;
            }
        }
    }
}