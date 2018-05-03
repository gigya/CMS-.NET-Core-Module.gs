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
                    UID = user.Name
                };
                return currentIdentity;
            }
        }

        public bool Exists(string userName)
        {
            var fullName = Context.Domain.GetFullName(userName);

            return User.Exists(fullName);
        }

        public User Login(string userName, string password)
        {
            var accountName = string.Empty;
            var domain = Context.Domain;
            if (domain != null)
            {
                accountName = domain.GetFullName(userName);
            }

            var result = AuthenticationManager.Login(accountName, password);
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

        public void RegisterUser(string email, string password, string profileId)
        {
            Assert.ArgumentNotNullOrEmpty(email, nameof(email));
            Assert.ArgumentNotNullOrEmpty(password, nameof(password));

            var fullName = Context.Domain.GetFullName(email);
            Assert.IsNotNullOrEmpty(fullName, "Can't retrieve full userName");

            var user = User.Create(fullName, password);
            user.Profile.Email = email;
            if (!string.IsNullOrEmpty(profileId))
            {
                user.Profile.ProfileItemId = profileId;
            }

            user.Profile.Save();
            _pipelineService.RunRegistered(user);

            this.Login(email, password);
        }
    }
}