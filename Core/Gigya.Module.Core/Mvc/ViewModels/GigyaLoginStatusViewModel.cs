using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core.Mvc.ViewModels
{
    public class GigyaLoginStatusViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsDesignMode { get; set; }
        public string ErrorMessage { get; set; }
        public string LoggedInRedirectUrl { get; set; }
        public string LogoutUrl { get; set; }
    }
}