using Gigya.Sitefinity.Module.DeleteSync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Tests.Providers
{
    public class FakeUserManager : IUserManager
    {
        public void Delete(User user)
        {
            
        }

        public User GetUser(string id)
        {
            if (id == "valid")
            {
                return new User
                {
                };
            }

            return null;
        }

        public void SaveChanges()
        {
            
        }
    }
}
