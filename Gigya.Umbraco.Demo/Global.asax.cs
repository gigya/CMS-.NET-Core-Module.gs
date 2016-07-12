using Gigya.Module.Core.Connector.Events;
using Gigya.Umbraco.Module.Connector.Helpers;
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

            GigyaMembershipHelper.GettingGigyaValue += GigyaMembershipHelper_GettingGigyaValue;

            MemberService.Saved += MemberService_Saved;
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