using Gigya.Module.Core.Data;
using Gigya.Umbraco.Module.v621.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace Gigya.Umbraco.Module.v621
{
    public static class ModuleInstaller
    {
        public static void PreApplicationStart()
        {
            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
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
                if (!db.TableExist("gigya_settings"))
                {
                    db.CreateTable<GigyaUmbracoModuleSettings>(false);
                }
            }
        }
    }
}
