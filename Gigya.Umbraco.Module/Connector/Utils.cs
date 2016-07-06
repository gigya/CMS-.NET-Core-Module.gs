using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Gigya.Umbraco.Module.Connector
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
    }
}
