using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Providers
{
    public class SitefinityProfileManager : IProfileManager
    {
        protected readonly UserProfileManager _manager = UserProfileManager.GetManager();

        public void Delete(SitefinityProfile profile)
        {
            _manager.Delete(profile);
        }

        public TUserProfileType GetUserProfile<TUserProfileType>(User user) where TUserProfileType : UserProfile
        {
            return _manager.GetUserProfile<TUserProfileType>(user);
        }

        public void SaveChanges()
        {
            _manager.SaveChanges();
        }
    }
}
