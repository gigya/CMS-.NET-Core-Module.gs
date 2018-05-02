using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

using umbraco.BusinessLogic.Actions;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Gigya.Umbraco.Module.Trees
{
    [Tree("gigya", "gigyaTree", "Gigya Settings")]
    [PluginController("Gigya")]
    public class GigyaTreeController : TreeController
    {
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            var globalSettingsNode = CreateTreeNode("-1", "-2", queryStrings, "Global Settings");
            nodes.Add(globalSettingsNode);

            var rootNode = new DynamicNode(-1);
            var homepageNodes = rootNode.Descendants(Constants.HomepageAlias);

            foreach (var node in homepageNodes)
            {
                nodes.Add(CreateTreeNode(node.Id.ToString(), "-2", queryStrings, node.Name));
            }

            return nodes;
        }
    }
}
