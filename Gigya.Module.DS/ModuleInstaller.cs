using Gigya.Module.Core.Connector.Events;
using Gigya.Module.DS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigya.Module.DS
{
    public static class ModuleInstaller
    {
        public static void PreApplicationStart()
        {
            GigyaEventHub.Instance.GetAccountInfoCompleted += GigyaEventHub_GetAccountInfoCompleted;
        }

        private static void GigyaEventHub_GetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            var helper = new GigyaDsHelper(e.Settings, e.Logger);
            e.GigyaModel = helper.Merge(e.GigyaModel, e.MappingFields);
        }
    }
}
