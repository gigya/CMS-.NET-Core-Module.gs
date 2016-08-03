using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Gigya.Module.Core.Connector.Common
{
    public static class DynamicUtils
    {
        public static T GetValue<T>(dynamic model, string key)
        {
            if (model == null)
            {
                return default(T);
            }

            var properties = (IDictionary<string, object>)model;
            var keySplit = key.Split('.');
            var firstProperty = keySplit[0];
            var firstPropertyNameOnly = Regex.Replace(firstProperty, @"\[[\d]+\]$", string.Empty);

            if (keySplit.Length == 1)
            {
                if (properties.ContainsKey(firstPropertyNameOnly))
                {
                    return GetPropertyValue(model, firstProperty, firstPropertyNameOnly);
                }
                return default(T);
            }

            if (!properties.ContainsKey(firstPropertyNameOnly))
            {
                return default(T);
            }

            return GetValue<T>(GetPropertyValue(properties, firstProperty, firstPropertyNameOnly), key.Substring(firstProperty.Length + 1));
        }

        /// <summary>
        /// Caters for arrays.
        /// </summary>
        private static dynamic GetPropertyValue(IDictionary<string, object> model, string dynamicProperty, string propertyNameOnly)
        {
            var value = model[propertyNameOnly];
            if (value == null)
            {
                return null;
            }

            var list = value as IEnumerable<object>;
            if (list != null)
            {
                var indexMatches = Regex.Match(dynamicProperty, @"\[([\d]+)\]$");
                if (indexMatches.Groups.Count != 2)
                {
                    return null;
                }

                var index = Convert.ToInt32(indexMatches.Groups[1].Value);
                if (index < 0 || index > list.Count())
                {
                    return null;
                }
                return list.ElementAt(index);
            }

            return value;
        }
    }
}