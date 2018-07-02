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
        private static readonly ID _gigyaFacetId = new ID(A.Constants.FacetKeys.GigyaFacetId);

        public string[] FacetPath(Item child, ID rootToStopAt)
        {
            var result = new List<string>();

            if (child.Parent != null && child.Parent.ID == _gigyaFacetId)
            {
                result.Add(A.Constants.FacetKeys.Gigya);
                result.Add("Entries");
                result.Add(child.Name);
                result.Add("Values");
                return result.ToArray();
            }
            
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