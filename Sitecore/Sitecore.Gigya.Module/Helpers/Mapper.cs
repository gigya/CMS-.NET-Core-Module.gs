using Gigya.Module.Core.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SC = Sitecore.Gigya.Module.Models;

using Core = Gigya.Module.Core;
using Gigya.Module.Core.Connector.Models;
using Sitecore.Data.Items;
using Sitecore.Gigya.Module.Controllers;
using Sitecore.Gigya.Module.Models;
using A = Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Module.Helpers
{
    public class Mapper
    {
        public static SC.GigyaSettingsViewModel Map(Core.Mvc.Models.GigyaSettingsViewModel source)
        {
            return new SC.GigyaSettingsViewModel
            {
                ApiKey = source.ApiKey,
                DataCenter = source.DataCenter,
                DebugMode = source.DebugMode,
                ErrorMessage = source.ErrorMessage,
                GigyaScriptPath = source.GigyaScriptPath,
                Id = source.Id,
                IsGetInfoRequired = source.IsGetInfoRequired,
                IsLoggedIn = source.IsLoggedIn,
                LoggedInRedirectUrl = source.LoggedInRedirectUrl,
                LogoutUrl = source.LogoutUrl,
                RenderScript = source.RenderScript,
                Settings = source.Settings,
                SettingsJson = source.SettingsJson,
                EnableSSOToken = source.EnableSSOToken
            };
        }

        public static MappingField Map(Item item)
        {
            return new MappingField
            {
                CmsFieldName = item.Fields[Constants.Fields.MappingFields.SitecoreProperty].Value,
                GigyaFieldName = item.Fields[Constants.Fields.MappingFields.GigyaProperty].Value,
            };
        }

        public static A.MappingFieldGroup MapMappingFieldGroup(Item item)
        {
            var facet = new A.MappingFieldGroup();

            foreach (Item child in item.Children)
            {
                var templateId = child.TemplateID.ToString();
                switch (templateId)
                {
                    case Constants.Templates.xDB.IdValues.xDBContactPersonalInfo:
                        facet.PersonalInfoMapping = MapContactPersonalInfo(child);
                        break;
                    case Constants.Templates.xDB.IdValues.xDBContactPhoneNumbers:
                        facet.PhoneNumbersMapping = MapContactPhoneNumbers(child);
                        break;
                    case Constants.Templates.xDB.IdValues.xDBContactEmailAddresses:
                        facet.EmailAddressesMapping = MapContactEmailAddresses(child);
                        break;
                }
            }

            return facet;
        }

        private static A.ContactPersonalInfoMapping MapContactPersonalInfo(Item item)
        {
            return new A.ContactPersonalInfoMapping
            {
                Key = C.FacetKeys.Personal,
                FirstName = item.Fields[nameof(A.ContactPersonalInfoMapping.FirstName)]?.Value,
                Surname = item.Fields[nameof(A.ContactPersonalInfoMapping.Surname)]?.Value,
                BirthDate = item.Fields[nameof(A.ContactPersonalInfoMapping.BirthDate)]?.Value,
                Gender = item.Fields[nameof(A.ContactPersonalInfoMapping.Gender)]?.Value,
                JobTitle = item.Fields[nameof(A.ContactPersonalInfoMapping.JobTitle)]?.Value,
                MiddleName = item.Fields[nameof(A.ContactPersonalInfoMapping.MiddleName)]?.Value,
                Nickname = item.Fields[nameof(A.ContactPersonalInfoMapping.Nickname)]?.Value,
                Suffix = item.Fields[nameof(A.ContactPersonalInfoMapping.Suffix)]?.Value,
                Title = item.Fields[nameof(A.ContactPersonalInfoMapping.Title)]?.Value
            };
        }

        private static A.ContactPhoneNumbersMapping MapContactPhoneNumbers(Item item)
        {
            var field = new A.ContactPhoneNumbersMapping
            {
                Key = C.FacetKeys.PhoneNumbers,
                Preferred = item.Fields[nameof(A.ContactPhoneNumbersMapping.Preferred)]?.Value
            };

            if (item.Children.Any())
            {
                field.Entries = item.Children.Select(i => new A.ContactPhoneNumberMapping
                {
                    Key = i.Fields[nameof(A.ContactPhoneNumberMapping.Key)]?.Value,
                    CountryCode = i.Fields[nameof(A.ContactPhoneNumberMapping.CountryCode)]?.Value,
                    Number = i.Fields[nameof(A.ContactPhoneNumberMapping.Number)]?.Value,
                    Extension = i.Fields[nameof(A.ContactPhoneNumberMapping.Extension)]?.Value
                }).ToList();
            }

            return field;
        }

        private static A.ContactEmailAddressesMapping MapContactEmailAddresses(Item item)
        {
            var field = new A.ContactEmailAddressesMapping
            {
                Key = C.FacetKeys.Emails,
                Preferred = item.Fields[nameof(A.ContactEmailAddressesMapping.Preferred)]?.Value
            };

            if (item.Children.Any())
            {
                field.Entries = item.Children.Select(i => new A.ContactEmailAddressMapping
                {
                    Key = i.Fields[nameof(A.ContactEmailAddressMapping.Key)]?.Value,
                    SmtpAddress = i.Fields[nameof(A.ContactEmailAddressMapping.SmtpAddress)]?.Value,
                    BounceCount = i.Fields[nameof(A.ContactEmailAddressMapping.BounceCount)]?.Value
                }).ToList();
            }

            return field;
        }

        public static AutocompleteSuggestion Map(AccountSchemaProperty item)
        {
            return new AutocompleteSuggestion
            {
                Data = item.Name,
                Value = item.Name
            };
        }
    }
}