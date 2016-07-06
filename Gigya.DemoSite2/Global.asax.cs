using Gigya.Module.Connector.Events;
using Gigya.Module.Connector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        private void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "RegisterRoutes")
            {
                //EventHub.Subscribe<IMapGigyaFieldEvent>(GigyaMembershipHelper_MapGigyaField);
            }
        }

        private void GigyaMembershipHelper_MapGigyaField(IMapGigyaFieldEvent @event)
        {
            switch (@event.GigyaFieldName)
            {
                case "profile.country":
                    {
                        if (@event.GigyaValue != null)
                        {
                            var value = @event.GigyaValue.ToString();
                            @event.GigyaValue = value == "United Kingdom" ? 826 : 0;
                        }
                    }
                    return;
            }

            var profile = @event.GigyaModel.profile;
            switch (@event.SitefinityFieldName)
            {
                case "BirthDate":
                    if (@event.GigyaValue != null)
                    {
                        @event.GigyaValue = new DateTime(Convert.ToInt32(profile.birthYear), Convert.ToInt32(profile.birthMonth), Convert.ToInt32(profile.birthDay));
                    }
                    return;
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}