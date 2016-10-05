using Gigya.Module.Core.Connector.Events;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.DS.Helpers;
using Gigya.Umbraco.Module.Connector;
using Gigya.Umbraco.Module.DS.Data;
using Gigya.Umbraco.Module.DS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace Gigya.Umbraco.Module.DS
{
    public static class ModuleInstaller
    {
        public static void PreApplicationStart()
        {
            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
            GigyaEventHub.Instance.GetAccountInfoCompleted += GigyaEventHub_GetAccountInfoCompleted;
        }

        private static void UmbracoApplicationBase_ApplicationInit(object sender, EventArgs e)
        {
            InitDb();
        }

        private static void InitDb()
        {
            if (UmbracoContext.Current != null)
            {
                var db = UmbracoContext.Current.Application.DatabaseContext.Database;
                if (!db.TableExist("gigya_ds_settings"))
                {
                    db.CreateTable<GigyaUmbracoModuleDsSettings>(false);
                }

                if (!db.TableExist("gigya_ds_mapping"))
                {
                    db.CreateTable<GigyaUmbracoDsMapping>(false);
                }
            }
        }

        /// <summary>
        /// Event that is called whenever the user's data is retrieved with getAccountInfo.
        /// This method retrieves the ds data and merges it with the getAccountInfo object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GigyaEventHub_GetAccountInfoCompleted(object sender, GetAccountInfoCompletedEventArgs e)
        {
            var logger = new Logger(new UmbracoLogger());
            var settingsHelper = new GigyaUmbracoDsSettingsHelper(logger);

            var settings = settingsHelper.Get(e.Settings.Id.ToString());
            var helper = new GigyaDsHelper(e.Settings, e.Logger, settings);
            e.GigyaModel = helper.Merge(e.GigyaModel, e.MappingFields);
        }
    }
}
