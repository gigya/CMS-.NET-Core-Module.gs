using Gigya.Module.Core.Connector.Common;
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Gigya.DependencyInjection;
using Sitecore.Gigya.Extensions.Abstractions.Analytics.Models;
using Sitecore.Gigya.Extensions.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Extensions.Services
{
    public class ContactProfileService : IContactProfileService
    {
        public IContactProfileProvider ContactProfileProvider { get; private set; }

        public ContactProfileService(IContactProfileProvider contactProfileProvider)
        {
            ContactProfileProvider = contactProfileProvider;
        }

        public void UpdateFacets(dynamic gigyaModel, MappingFieldGroup mappingFields)
        {
            UpdatePersonalFacet(gigyaModel, mappingFields.PersonalInfoMapping);
            UpdatePhoneNumbersFacet(gigyaModel, mappingFields.PhoneNumbersMapping);
            UpdateEmailAddressesFacet(gigyaModel, mappingFields.EmailAddressesMapping);

            ContactProfileProvider.Flush();
        }

        //private void UpdateFacet(IFacet facet, dynamic gigyaModel, List<MappingField> mappingFields)
        //{
        //    try
        //    {
        //        if (facet == null)
        //        {
        //            return;
        //        }

        //        var properties = facet.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanWrite).ToList();

        //        foreach (var mappingField in mappingFields)
        //        {
        //            var property = properties.FirstOrDefault(i => i.Name == mappingField.CmsFieldName);
        //            if (property == null)
        //            {
        //                Log.Warn(string.Format("[Gigya]: The '{0}' facet property is not available.", mappingField.CmsFieldName), this);
        //                continue;
        //            }

        //            var value = DynamicUtils.GetValue<object>(gigyaModel, mappingField.GigyaFieldName);
        //            property.SetValue(facet, value);
        //        }
        //    }
        //    catch (FacetNotAvailableException ex)
        //    {
        //        Log.Warn(string.Join("[Gigya]: The '{0}' facet is not available.", facet.GetType()), ex);
        //    }
        //}

        private void UpdatePersonalFacet(dynamic gigyaModel, ContactPersonalInfoMapping mapping)
        {
            if (mapping == null)
            {
                return;
            }

            try
            {
                var facet = ContactProfileProvider.PersonalInfo;

                facet.BirthDate = DynamicUtils.GetValue<DateTime?>(gigyaModel, mapping.BirthDate);
                facet.FirstName = DynamicUtils.GetValue<string>(gigyaModel, mapping.FirstName);
                facet.Gender = DynamicUtils.GetValue<string>(gigyaModel, mapping.Gender);
                facet.JobTitle = DynamicUtils.GetValue<string>(gigyaModel, mapping.JobTitle);
                facet.MiddleName = DynamicUtils.GetValue<string>(gigyaModel, mapping.MiddleName);
                facet.Nickname = DynamicUtils.GetValue<string>(gigyaModel, mapping.Nickname);
                facet.Suffix = DynamicUtils.GetValue<string>(gigyaModel, mapping.Suffix);
                facet.Surname = DynamicUtils.GetValue<string>(gigyaModel, mapping.Surname);
                facet.Title = DynamicUtils.GetValue<string>(gigyaModel, mapping.Title);
            }
            catch (FacetNotAvailableException ex)
            {
                Log.Warn("The 'Personal' facet is not available.", ex);
            }
        }

        private void UpdatePhoneNumbersFacet(dynamic gigyaModel, ContactPhoneNumbersMapping mapping)
        {
            if (mapping == null)
            {
                return;
            }

            try
            {
                var facet = ContactProfileProvider.PhoneNumbers;

                facet.Preferred = DynamicUtils.GetValue<string>(gigyaModel, mapping.Preferred);
                
                if (mapping.Entries != null)
                {
                    foreach (var entryMapping in mapping.Entries)
                    {
                        IPhoneNumber phoneNumber;
                        if (!facet.Entries.Contains(entryMapping.Key))
                        {
                            phoneNumber = facet.Entries.Create(entryMapping.Key);
                        }
                        else
                        {
                            phoneNumber = facet.Entries[entryMapping.Key];
                        }

                        phoneNumber.CountryCode = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.CountryCode);
                        phoneNumber.Extension = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Extension);
                        phoneNumber.Number = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.Number);
                    }
                }
            }
            catch (FacetNotAvailableException ex)
            {
                Log.Warn("The 'Personal' facet is not available.", ex);
            }
        }

        private void UpdateEmailAddressesFacet(dynamic gigyaModel, ContactEmailAddressesMapping mapping)
        {
            if (mapping == null)
            {
                return;
            }

            try
            {
                var facet = ContactProfileProvider.Emails;

                facet.Preferred = DynamicUtils.GetValue<string>(gigyaModel, mapping.Preferred);

                if (mapping.Entries != null)
                {
                    foreach (var entryMapping in mapping.Entries)
                    {
                        IEmailAddress emailAddress;
                        if (!facet.Entries.Contains(entryMapping.Key))
                        {
                            emailAddress = facet.Entries.Create(entryMapping.Key);
                        }
                        else
                        {
                            emailAddress = facet.Entries[entryMapping.Key];
                        }

                        emailAddress.SmtpAddress = DynamicUtils.GetValue<string>(gigyaModel, entryMapping.SmtpAddress);
                        emailAddress.BounceCount = DynamicUtils.GetValue<int>(gigyaModel, entryMapping.BounceCount);
                    }
                }
            }
            catch (FacetNotAvailableException ex)
            {
                Log.Warn("The 'Personal' facet is not available.", ex);
            }
        }
    }
}