using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Gigya.Umbraco.Module.v621.Connector
{
    public static class Utils
    {
        public static IPublishedContent HomepageNode(IPublishedContent currentNode)
        {
            if (currentNode == null || currentNode.DocumentTypeAlias == Constants.HomepageAlias || currentNode.Parent == null)
            {
                return currentNode;
            }

            return HomepageNode(currentNode.Parent);
        }

        public static string UmbracoMacroProperty(IDictionary<string, object> properties, string key)
        {
            object value;
            if (!properties.TryGetValue(key, out value) || value == null)
            {
                return null;
            }

            // umbraco stores null as "null"
            var strValue = value.ToString();
            if (string.IsNullOrEmpty(strValue) || strValue == "null")
            {
                return null;
            }

            return strValue;
        }
    }
}
