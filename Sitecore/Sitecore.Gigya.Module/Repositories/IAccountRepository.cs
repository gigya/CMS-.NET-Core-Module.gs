using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = Sitecore;

namespace Sitecore.Gigya.Module.Repositories
{
    public interface IAccountRepository
    {
        SC.Security.Accounts.User Register(string username, string email, string password, bool persistent, string profileId);
        bool Exists(string userName);
        void Logout();
        SC.Security.Accounts.User GetActiveUser();
        SC.Security.Accounts.User Login(string userName, bool persistent);
        CurrentIdentity CurrentIdentity { get; }
    }
}
