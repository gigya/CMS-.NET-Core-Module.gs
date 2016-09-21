using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Gigya.Module.Core.Connector.Common
{
    public static class DynamicUtils
    {
        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>)kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>)item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }

        /// <summary>
        /// Merges updated object with source.   If the same property already exists in the source it will be overridden. 
        /// </summary>
        /// <param name="source">Source dynamic object.</param>
        /// <param name="updated">Dynamic object containing updated values.</param>
        /// <returns>A new object with the merged properties.</returns>
        public static ExpandoObject Merge(ExpandoObject source, ExpandoObject updated)
        {            
            var sourceProperties = (IDictionary<string, object>)source;
            var updatedProperties = (IDictionary<string, object>)updated;
            return Merge(sourceProperties, updatedProperties);
        }

        /// <summary>
        /// Merges updated object with source.   If the same property already exists in the source it will be overridden. 
        /// </summary>
        /// <param name="source">Source dynamic object.</param>
        /// <param name="updated">Dynamic object containing updated values.</param>
        /// <returns>A new object with the merged properties.</returns>
        public static ExpandoObject Merge(IDictionary<string, object> source, IDictionary<string, object> updated)
        {
            var result = new Dictionary<string, object>();

            foreach (var kv in source.Concat(updated))
            {
                result[kv.Key] = kv.Value;

                if (source.ContainsKey(kv.Key) && updated.ContainsKey(kv.Key))
                {
                    var sourceValue = source[kv.Key] as ExpandoObject;
                    var updatedValue = updated[kv.Key] as ExpandoObject;

                    if (sourceValue != null && updatedValue != null)
                    {
                        // same property exists in both so we need to merge
                        result[kv.Key] = Merge(sourceValue, updatedValue);
                    }
                }
            }

            return result.ToExpando();
        }

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
                if (index < 0 || index >= list.Count())
                {
                    return null;
                }
                return list.ElementAt(index);
            }

            return value;
        }
    }
}