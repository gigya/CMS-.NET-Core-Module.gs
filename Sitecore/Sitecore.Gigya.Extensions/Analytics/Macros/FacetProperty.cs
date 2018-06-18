using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.RuleMacros;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Sitecore.Gigya.Extensions.Analytics.Macros
{
    public class FacetProperty : IRuleMacro
    {
        public void Execute(XElement element, string name, UrlString parameters, string value)
        {
            Assert.ArgumentNotNull((object)element, nameof(element));
            Assert.ArgumentNotNull((object)name, nameof(name));
            Assert.ArgumentNotNull((object)parameters, nameof(parameters));
            Assert.ArgumentNotNull((object)value, nameof(value));

            SelectItemOptions selectItemOptions = new SelectItemOptions();
            Item obj1 = (Item)null;
            if (!string.IsNullOrEmpty(value))
            {
                obj1 = Client.ContentDatabase.GetItem(value);
            }

            string path = XElement.Parse(element.ToString()).FirstAttribute.Value;
            if (!string.IsNullOrEmpty(path))
            {
                Item obj2 = Client.ContentDatabase.GetItem(path);
                if (obj2 != null)
                {
                    selectItemOptions.FilterItem = obj2;
                }
            }

            selectItemOptions.Root = Client.ContentDatabase.GetItem(RuleIds.OperatorsFolder);
            selectItemOptions.SelectedItem = obj1 ?? (selectItemOptions.Root != null ? selectItemOptions.Root.Children.FirstOrDefault<Item>() : (Item)null);
            selectItemOptions.Title = "Select Comparison";
            selectItemOptions.Text = "Select the numerical comparison to use in this rule.";
            selectItemOptions.Icon = "applications/32x32/media_stop.png";
            selectItemOptions.ShowRoot = false;

            SheerResponse.ShowModalDialog(selectItemOptions.ToUrlString().ToString(), "1200px", "700px", "", true);
        }
    }
}