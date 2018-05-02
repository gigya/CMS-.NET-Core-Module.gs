using Gigya.Module.Core.Connector.Events;
using Gigya.Umbraco.Module.v621.Connector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace Gigya.Umbraco.Demo.v621
{
    public class Global : UmbracoApplication
    {
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            GigyaMembershipHelper.GettingGigyaValue += GigyaMembershipHelper_GettingGigyaValue;
        }

        private void GigyaMembershipHelper_GettingGigyaValue(object sender, MapGigyaFieldEventArgs e)
        {
            var profile = e.GigyaModel.profile;
            switch (e.CmsFieldName)
            {
                case "birthDate":
                    if (e.GigyaValue != null)
                    {
                        try
                        {
                            e.GigyaValue = new DateTime(Convert.ToInt32(profile.birthYear), Convert.ToInt32(profile.birthMonth), Convert.ToInt32(profile.birthDay));
                        }
                        catch
                        {
                            // log
                        }

                    }
                    return;
            }
        }
    }
}