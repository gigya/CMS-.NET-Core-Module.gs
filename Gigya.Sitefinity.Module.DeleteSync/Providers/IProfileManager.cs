using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Providers
{
    public interface IProfileManager
    {
        TUserProfileType GetUserProfile<TUserProfileType>(User user) where TUserProfileType : UserProfile;
        void Delete(SitefinityProfile profile);
        void SaveChanges();
    }
}
