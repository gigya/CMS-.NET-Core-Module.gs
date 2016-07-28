﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module
{
    public static class Constants
    {
        public class GigyaFields
        {
            public const string FirstName = "profile.firstName";
            public const string LastName = "profile.lastName";
            public const string Email = "profile.email";
            public const string UserId = "UID";
            public const string UserIdSignature = "UIDSignature";
            public const string SignatureTimestamp = "signatureTimestamp";
        }

        public class SitefinityFields
        {
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string Email = "Email";
            public const string UserId = "UserId";
        }

        public class Testing
        {
            public const string EmailWhichThrowsException = "loginexception@purestone.co.uk";
        }

        public class Roles
        {
            public const string GigyaAdmin = "Gigya Major Admin";
            public const string GigyaMinorAdmin = "Gigya Minor Admin";
        }

        public class Resources
        {
            public const string ClassId = "GigyaResources";
            public const string ErrorMessage = "ErrorMessage";

            public class FrontEnd
            {
                public class LoginStatus
                {
                    public const string LoggedInUserGreeting = "LoggedInUserGreeting";
                }
            }

            public class Designer
            {
                public class SettingsWidget
                {
                    public const string NothingToEditText = "SettingsWidgetNothingToEdit";
                }

                public class LoginStatus
                {
                    public const string LogoutPageLabel = "LogoutPageLabel";
                    public const string RedirectPageLabel = "RedirectPageLabel";
                }
            }
        }
    }
}