using NSubstitute;
using Ploeh.AutoFixture.AutoNSubstitute;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.AutoFixture;
using Sitecore.FakeDb.Sites;
using Sitecore.Gigya.Module.Controllers;
using Sitecore.Gigya.Module.Repositories;
using Sitecore.Gigya.Testing;
using Sitecore.Gigya.Testing.Attributes;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ModelBinding;
using Xunit;

namespace Sitecore.Gigya.Module.Tests.Controllers
{
    public class AccountControllerTests
    {
        public AccountControllerTests()
        {
            HttpContext.Current = HttpContextMockFactory.Create();
            //var dictionaryPhraseRepository = Substitute.For<IDictionaryPhraseRepository>();
            //dictionaryPhraseRepository.Get(Arg.Any<string>(), Arg.Any<string>()).Returns(x => x[1] as string);
            //HttpContext.Current.Items["DictionaryPhraseRepository.Current"] = dictionaryPhraseRepository;
        }

        [Theory]
        [AutoDbData]
        public void Logout_ShouldCallSitecoreLogout(Database db, [Content] DbItem item, IAccountRepository repo)
        {
            var fakeSite = new FakeSiteContext(new StringDictionary
                                               {
                                                   {"rootPath", "/sitecore/content"},
                                                   {"startItem", item.Name}
                                               }) as SiteContext;
            fakeSite.Database = db;

            using (new SiteContextSwitcher(fakeSite))
            {
                var ctrl = new AccountController(repo, null, null);
                ctrl.Logout();
                repo.Received(1).Logout();
            }
        }

        //public void EditProfile_CustomProperties()
        //{

        //}

        //[Theory]
        //[AutoDbData]
        //public void EditProfilePost_InvalidData_ShouldReturnModelErrors(FakeSiteContext siteContext, ModelStateDictionary modelState, string profileItemId, IEnumerable<string> interests, [Substitute] EditProfile editProfile, IUserProfileService userProfileService)
        //{
        //    var user = Substitute.For<User>("extranet/John", true);
        //    user.Profile.Returns(Substitute.For<UserProfile>());
        //    user.Profile.ProfileItemId = profileItemId;
        //    userProfileService.GetUserDefaultProfileId().Returns(profileItemId);
        //    userProfileService.GetInterests().Returns(interests);
        //    userProfileService.ValidateProfile(Arg.Any<EditProfile>(), Arg.Do<ModelStateDictionary>(x => x.AddModelError("key", "error"))).Returns(false);

        //    using (new SiteContextSwitcher(siteContext))
        //    using (new UserSwitcher(user))
        //    {
        //        var accounController = new AccountController(null, null, null, null, userProfileService, null);
        //        var result = accounController.EditProfile(editProfile);
        //        result.Should().BeOfType<ViewResult>().Which.ViewData.ModelState.Should().ContainKey("key").WhichValue.Errors.Should().Contain(e => e.ErrorMessage == "error");
        //    }
        //}
    }
}
