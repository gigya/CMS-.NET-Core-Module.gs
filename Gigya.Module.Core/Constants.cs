using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gigya.Module.Core
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

        public class Testing
        {
            public const string EmailWhichThrowsException = "loginexception@purestone.co.uk";
        }

        public class DataCenter
        {
            public const string Other = "Other";
            public static readonly string[] DataCenters = new string[] { "us1", "eu1", "au1", "ru1" };
        }

        public class Languages
        {
            public const string Other = "Other";
            public const string Auto = "auto";
            public const string AutoName = "Auto";
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