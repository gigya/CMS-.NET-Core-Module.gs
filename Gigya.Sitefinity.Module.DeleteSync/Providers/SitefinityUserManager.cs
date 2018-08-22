using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Providers
{
    public class SitefinityUserManager : IUserManager
    {
        protected readonly UserManager _manager = UserManager.GetManager();

        public void Delete(User user)
        {
            _manager.Delete(user);
        }

        public User GetUser(string id)
        {
            return _manager.GetUser(id);
        }

        public void SaveChanges()
        {
            _manager.SaveChanges();
        }
    }
}
