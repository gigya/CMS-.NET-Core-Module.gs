using Gigya.Module.Connector.Events;
using Gigya.Module.Connector.Helpers;
using Gigya.Module.Connector.Logging;
using Gigya.Sitefinity.Module.DS.Helpers;
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

        /// <summary>
        /// Sample method to manually retrieve DS data from Gigya.
        /// </summary>
        /// <remarks>
        /// This method retrieves Gigya settings from Sitefinity.
        /// If you want to use your own settings this can be done by passing your own settings models into the GigyaDsHelper constructor.
        /// </remarks>
        private void ManuallyRetrieveDsData()
        {
            // create a new helper Gigya DS Helper to retrieve DS data from Gigya
            var dsHelper = GigyaDsHelperFactory.Instance();

            // retrieve DS data for a user who's id is userIdValue
            var dsData = dsHelper.GetOrSearch("userIdValue");
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
            //switch (@event.GigyaFieldName)
            //{
            //    case "profile.country":
            //        {
            //            if (@event.GigyaValue != null)
            //            {
            //                var value = @event.GigyaValue.ToString();
            //                @event.GigyaValue = value == "United Kingdom" ? 826 : 0;
            //            }
            //        }
            //        return;
            //}

            var profile = @event.GigyaModel.profile;
            switch (@event.SitefinityFieldName)
            {
                case "BirthOfDate":
                    if (@event.GigyaValue != null)
                    {
                        try
                        {
                            @event.GigyaValue = new DateTime(Convert.ToInt32(profile.birthYear), Convert.ToInt32(profile.birthMonth), Convert.ToInt32(profile.birthDay));
                        }
                        catch { }
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