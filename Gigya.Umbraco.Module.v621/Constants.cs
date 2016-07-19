using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Gigya.Umbraco.Module.v621
{
    public static class Constants
    {
        public const string ModuleVersion = "1.0.0.0";
        public static readonly string HomepageAlias = ConfigurationManager.AppSettings["umbracoHomepageAlias"] ?? "umbHomepage";
        public static readonly string MemberTypeAlias = ConfigurationManager.AppSettings["umbracoMemberTypeAlias"] ?? "Member";

        public class GigyaFields
        {
            public const string FirstName = "profile.firstName";
            public const string LastName = "profile.lastName";
            public const string Email = "profile.email";
            public const string UserId = "UID";
            public const string UserIdSignature = "UIDSignature";
            public const string SignatureTimestamp = "signatureTimestamp";
        }

        public class CmsFields
        {
            public const string Email = "email";
            public const string Name = "name";
            public const string Username = "username";
        }

        public class Testing
        {
            public const string EmailWhichThrowsException = "loginexception@purestone.co.uk";
        }

        public class Roles
        {
            public const string GigyaAdmin = "Gigya Major Admin";
            public const string Admin = "Administrator";
        }

        public class UserTypes
        {
            public const string Admin = "Administrators";
        }
    }
}