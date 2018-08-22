using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Security.Model;

namespace Gigya.Sitefinity.Module.DeleteSync.Providers
{
    public interface IUserManager
    {
        User GetUser(string id);
        void Delete(User user);
        void SaveChanges();
    }
}
