using Gigya.Module.Core.Mvc.Models;
using Sitecore.Diagnostics;
using Sitecore.Gigya.Module.Pipelines;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SC = Sitecore;

namespace Sitecore.Gigya.Module.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PipelineService _pipelineService;

        public AccountRepository(PipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public CurrentIdentity CurrentIdentity
        {
            get
            {
                var user = SC.Context.User;
                var currentIdentity = new CurrentIdentity
                {
                    IsAuthenticated = user.IsAuthenticated,
                    Name = user.Name,
                    UID = user.LocalName
                };
                return currentIdentity;
            }
        }

        public bool Exists(string userName)
        {
            var fullName = Context.Domain.GetFullName(userName);

            return User.Exists(fullName);
        }

        public User GetActiveUser()
        {
            var user = AuthenticationManager.GetActiveUser();
            return user;
        }

        public User Login(string userName, bool persistent)
        {
            var accountName = string.Empty;
            var domain = Context.Domain;
            if (domain != null)
            {
                accountName = domain.GetFullName(userName);
            }

            var result = AuthenticationManager.Login(accountName, persistent);
            if (!result)
            {
                return null;
            }

            var user = AuthenticationManager.GetActiveUser();
            _pipelineService.RunLoggedIn(user);
            return user;
        }

        public void Logout()
        {
            var user = AuthenticationManager.GetActiveUser();
            AuthenticationManager.Logout();
            if (user != null)
            {
                _pipelineService.RunLoggedOut(user);
            }
        }

        public User Register(string username, string email, string password, bool persistent, string profileId)
        {
            Assert.ArgumentNotNullOrEmpty(email, nameof(email));

            var fullName = Context.Domain.GetFullName(username);
            Assert.IsNotNullOrEmpty(fullName, "Can't retrieve full userName");

            var user = User.Create(fullName, password);
            user.Profile.Email = email;
            if (!string.IsNullOrEmpty(profileId))
            {
                user.Profile.ProfileItemId = profileId;
            }

            user.Profile.Save();
            _pipelineService.RunRegistered(user);

            return user;
        }
    }
}