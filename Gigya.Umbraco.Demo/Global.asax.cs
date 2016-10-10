using Gigya.Module.Core.Connector.Events;
using Gigya.Module.Core.Connector.Logging;
using Gigya.Module.Core.Data;
using Gigya.Module.DS.Helpers;
using Gigya.Umbraco.Module.Connector;
using Gigya.Umbraco.Module.Connector.Helpers;
using Gigya.Umbraco.Module.DS.Helpers;
using Gigya.Umbraco.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Gigya.Umbraco.Demo
{
    public class Global : UmbracoApplication
    {
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            // register event to be called after Gigya DS data has been merged with 
            GigyaEventHub.Instance.AccountInfoMergeCompleted += Instance_AccountInfoMergeCompleted;
        }

        private void Instance_AccountInfoMergeCompleted(object sender, AccountInfoMergeCompletedEventArgs e)
        {
            // model representing Gigya DS data that has been merged with the getAccountInfo model
            dynamic gigyaAccountInfoWithDsDataMerged = e.GigyaModel;

            try
            {
                // assuming I have a ds field called ds.addressInfo.line1_s then I would use this code to change the value
                gigyaAccountInfoWithDsDataMerged.ds.addressInfo.line1_s = "first line of address";
            }
            catch(Exception ex)
            {
                e.Logger.Error("Error in Instance_AccountInfoMergeCompleted.", ex);
            }
        }

        /// <summary>
        /// Sample method to manually retrieve DS data from Gigya.
        /// </summary>
        /// <remarks>
        /// This method retrieves Gigya settings from Umbraco.
        /// If you want to use your own settings this can be done by passing your own settings models into the GigyaDsHelper constructor.
        /// </remarks>
        private void ManuallyRetrieveDsData()
        {
            // create a new Umbraco logger
            var logger = new Logger(new UmbracoLogger());

            // create a new Gigya Umbraco DS Settings helper for retrieving DS settings from the database (these are managed through Umbraco)
            var settingsHelper = new GigyaUmbracoDsSettingsHelper(logger);
            
            // gets the DS settings for the current site (this is done by finding the homepage from the current Umbraco page) 
            var dsSettings = settingsHelper.GetForCurrentSite();

            // create a new Gigya Settings helper for getting core module settings
            var coreSettingsHelper = new GigyaSettingsHelper();

            // get core settings for the current site (this is done by finding the homepage from the current Umbraco page) 
            var coreSettings = coreSettingsHelper.GetForCurrentSite(true);

            // create a new helper Gigya DS Helper to retrieve DS data from Gigya
            var dsHelper = new GigyaDsHelper(coreSettings, logger, dsSettings);

            // retrieve DS data for a user who's id is userIdValue
            var dsData = dsHelper.GetOrSearch("userIdValue");
        }

        private void Instance_GettingGigyaValue(object sender, MapGigyaFieldEventArgs e)
        {
            //GigyaEventHub.Instance.GettingGigyaValue += Instance_GettingGigyaValue;
            //MemberService.Saved += MemberService_Saved;

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

        private void MemberService_Saved(IMemberService sender, global::Umbraco.Core.Events.SaveEventArgs<global::Umbraco.Core.Models.IMember> e)
        {
            var memberService = ApplicationContext.Current.Services.MemberService;
            foreach (var member in e.SavedEntities)
            {
                // this is a brand new member object
                if (member.IsNewEntity())
                {
                    // add member group for example:
                    memberService.AssignRole(member.Id, "Authenticated");
                }
            }
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