using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;

namespace Gigya.Module.Core.Connector.Helpers
{
    public interface IGigyaMembershipHelper : IGigyaMembershipHelper<GigyaModuleSettings>
    {

    }

    public interface IGigyaMembershipHelper<T> where T : GigyaModuleSettings
    {
        void LoginOrRegister(LoginModel model, T settings, ref LoginResponseModel response);
        bool Login(string gigyaUid, T settings);
        void UpdateProfile(LoginModel model, T settings, ref LoginResponseModel response);
        void Logout();
    }
}