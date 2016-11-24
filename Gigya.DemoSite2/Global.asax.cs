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

        private void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "RegisterRoutes")
            {
                EventHub.Subscribe<IMapGigyaFieldEvent>(GigyaMembershipHelper_MapGigyaField);
            }

            // register event to be called after Gigya DS data has been merged with 
            Gigya.Module.Core.Connector.Events.GigyaEventHub.Instance.AccountInfoMergeCompleted += Instance_AccountInfoMergeCompleted;

            Gigya.Module.Core.Connector.Events.GigyaEventHub.Instance.FetchDSCompleted += Instance_FetchDSCompleted;
        }

        private static void Instance_FetchDSCompleted(object sender, Gigya.Module.Core.Connector.Events.FetchDSCompletedEventArgs e)
        {
            // manipulate DS data if required e.g.:

            try
            {
                e.GigyaModel.ds.dsType.field = "updated";
            }
            catch (Exception ex)
            {
                e.Logger.Error("Failed to update DS data after fetch.", ex);
            }
        }

        private void Instance_AccountInfoMergeCompleted(object sender, Gigya.Module.Core.Connector.Events.AccountInfoMergeCompletedEventArgs e)
        {
            // model representing Gigya DS data that has been merged with the getAccountInfo model
            dynamic gigyaAccountInfoWithDsDataMerged = e.GigyaModel;

            try
            {
                // assuming I have a ds field called ds.addressInfo.line1_s then I would use this code to change the value
                gigyaAccountInfoWithDsDataMerged.ds.addressInfo.line1_s = "first line of address";
            }
            catch (Exception ex)
            {
                e.Logger.Error("Error in Instance_AccountInfoMergeCompleted.", ex);
            }
        }

        /// <summary>
        /// Sample method to manually retrieve DS data from Gigya.
        /// </summary>
        /// <remarks>
        /// This method retrieves Gigya settings from Sitefinity.
        /// If you want to use your own settings this can be done by passing your own settings models into the GigyaDsHelper constructor.
        /// </remarks>
        private dynamic ManuallyRetrieveDsData()
        {
            // create a new helper Gigya DS Helper to retrieve DS data from Gigya
            var dsHelper = GigyaDsHelperFactory.Instance();

            // retrieve DS data for a user who is logged in
            var dsData = dsHelper.GetOrSearchForCurrentUser();

            // get DS data for a user who's UID is UID
            var anotherSample = dsHelper.GetOrSearch("UID");
            
            // do further processing here
            return dsData;
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