using Sitecore.Gigya.Module.Helpers;
using Sitecore.Gigya.Module.Pipelines;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sitecore.Gigya.Module.Tests.Pipelines
{
    public class GetGigyaValueTest
    {
        public void Process(GigyaGetFieldEventArgs args)
        {
            // check that we can change the gigya value on the fly
            if (args.GigyaValue is string)
            {
                args.GigyaValue = new Random().Next(int.MaxValue).ToString();
            }
        }

        //[Theory]
        //public void ShouldBeAbleToChangeGigyaFieldValue(GigyaMembershipHelper gigyaMembershipHelper)
        //{
            
        //    //var accountRepository = new AccountRepository();
        //    //Logger = new Logger(new SitecoreLogger());
        //    //SettingsHelper = new Helpers.GigyaSettingsHelper();

        //    //_gigyaAccountHelper = new GigyaAccountHelper(SettingsHelper, Logger, null, _accountRepository);
        //    //var apiHelper = new GigyaApiHelper<SitecoreGigyaModuleSettings>(SettingsHelper, Logger);
        //    //MembershipHelper = new GigyaMembershipHelper(apiHelper, Logger, _gigyaAccountHelper, _accountRepository);
        //    //var helper = new GigyaMembershipHelper()

        //}
    }
}
