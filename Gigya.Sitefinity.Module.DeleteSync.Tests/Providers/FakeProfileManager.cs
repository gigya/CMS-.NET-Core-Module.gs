using Gigya.Sitefinity.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Tests.Providers
{
    public class FakeProfileManager : IProfileManager
    {
        public void Delete(SitefinityProfile profile)
        {
            
        }

        public TUserProfileType GetUserProfile<TUserProfileType>(User user) where TUserProfileType : UserProfile
        {
            return null;
        }

        public void SaveChanges()
        {
            
        }
    }
}
