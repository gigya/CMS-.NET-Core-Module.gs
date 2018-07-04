using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using A = Sitecore.Gigya.Extensions.Abstractions.Analytics;

namespace Sitecore.Gigya.Connector.Services
{
    public class SitecoreContentService
    {
        public string[] FacetPath(Item child, ID rootToStopAt)
        {
            var result = new List<string>();

            AddPath(result, child, rootToStopAt);
            result.Reverse();
            return result.ToArray();
        }

        private void AddPath(List<string> paths, Item item, ID rootToStopAt)
        {
            if (item == null || item.ID == rootToStopAt)
            {
                return;
            }

            paths.Add(item.Name);
            AddPath(paths, item.Parent, rootToStopAt);
        }
    }
}