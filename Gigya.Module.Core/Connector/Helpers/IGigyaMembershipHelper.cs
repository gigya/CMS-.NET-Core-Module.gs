using Gigya.Module.Core.Data;
using Gigya.Module.Core.Mvc.Models;

namespace Gigya.Module.Core.Connector.Helpers
{
    public interface IGigyaMembershipHelper
    {
        void LoginOrRegister(LoginModel model, IGigyaModuleSettings settings, ref LoginResponseModel response);
        bool Login(string gigyaUid, IGigyaModuleSettings settings);
        void UpdateProfile(LoginModel model, IGigyaModuleSettings settings, ref LoginResponseModel response);
        void Logout();
    }
}