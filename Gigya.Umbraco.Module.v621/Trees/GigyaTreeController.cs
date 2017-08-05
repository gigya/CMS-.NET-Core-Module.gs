using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Gigya.Umbraco.Module.v621.Trees
{
    [umbraco.businesslogic.Tree("gigya", "gigyaTree", "Gigya Settings")]
    [PluginController("Gigya")]
    public class GigyaTreeController : BaseTree
    {
        public GigyaTreeController(string application) : base(application)
        {
        }

        public override void Render(ref XmlTree tree)
        {
            if (this.NodeKey == string.Empty)
            {
                PopulateRootNodes(ref tree);
            }
        }

        public override void RenderJS(ref StringBuilder Javascript)
        {
            Javascript.Append(@"function gigyaInit(url) { parent.right.document.location.href = url; }");
        }

        protected override void CreateAllowedActions(ref List<IAction> actions)
        {
            actions.Clear();
        }

        /// <summary>
        /// Render the top Root Nodes.
        /// - Calendar -> The Root of all Calendars
        /// - Locations -> The Root of all Locations
        /// </summary>
        /// <param name="tree">The current tree</param>
        private void PopulateRootNodes(ref XmlTree tree)
        {
            XmlTreeNode xNode = CreateNode("Global Settings", -1);
            tree.Add(xNode);

            var rootNode = new DynamicNode(-1);
            var homepageNodes = rootNode.Descendants(Constants.HomepageAlias);

            foreach (var node in homepageNodes)
            {
                xNode = CreateNode(node.Name, node.Id);
                tree.Add(xNode);
            }
        }

        private XmlTreeNode CreateNode(string title, int id)
        {
            XmlTreeNode xNode = XmlTreeNode.Create(this);
            xNode.NodeID = id.ToString();
            xNode.Text = title;
            xNode.Action = "javascript:gigyaInit('/umbraco/plugins/gigya/edit.aspx?id=" + id + "')";
            xNode.Icon = "doc.gif";
            xNode.OpenIcon = "folder_o.gif";
            xNode.NodeType = "GigyaSettings";
            return xNode;
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.NodeType = "init" + TreeAlias;
            rootNode.NodeID = "-2";
            rootNode.Text = "Settings";
            rootNode.Menu.Clear();
            rootNode.Menu.AddRange(new List<IAction> { ActionRefresh.Instance });
        }
    }
}
