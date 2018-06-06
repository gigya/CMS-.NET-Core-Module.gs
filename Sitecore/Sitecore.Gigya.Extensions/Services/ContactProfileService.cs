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

        public void UpdateFacets(dynamic gigyaModel, List<MappingFieldGroup> mappingFields)
        {
            foreach (var group in mappingFields)
            {
                UpdateFacet(gigyaModel, group);
            }

            ContactProfileProvider.Flush();
        }

        private void UpdateFacet(dynamic gigyaModel, MappingFieldGroup group)
        {
            if (group.Fields == null || !group.Fields.Any())
            {
                return;
            }

            switch (group.FacetName)
            {
                case C.FacetKeys.Personal:
                    UpdateFacet(ContactProfileProvider.PersonalInfo, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.Addresses:
                    UpdateFacet(ContactProfileProvider.Addresses, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.CommunicationProfile:
                    UpdateFacet(ContactProfileProvider.CommunicationProfile, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.Emails:
                    UpdateFacet(ContactProfileProvider.Emails, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.PhoneNumbers:
                    UpdateFacet(ContactProfileProvider.PhoneNumbers, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.Picture:
                    UpdateFacet(ContactProfileProvider.Picture, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.Preferences:
                    UpdateFacet(ContactProfileProvider.Preferences, gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.Gigya:
                    UpdateFacet(ContactProfileProvider.Gigya, gigyaModel, group.Fields);
                    break;
            }
        }

        private void UpdateFacet(IFacet facet, dynamic gigyaModel, List<MappingField> mappingFields)
        {
            try
            {
                if (facet == null)
                {
                    return;
                }

                var properties = facet.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.CanWrite).ToList();

                foreach (var mappingField in mappingFields)
                {
                    var property = properties.FirstOrDefault(i => i.Name == mappingField.CmsFieldName);
                    if (property == null)
                    {
                        Log.Warn(string.Format("[Gigya]: The '{0}' facet property is not available.", mappingField.CmsFieldName), this);
                        continue;
                    }

                    var value = DynamicUtils.GetValue<object>(gigyaModel, mappingField.GigyaFieldName);
                    property.SetValue(facet, value);
                }
            }
            catch (FacetNotAvailableException ex)
            {
                Log.Warn(string.Join("[Gigya]: The '{0}' facet is not available.", facet.GetType()), ex);
            }
        }

        //private void UpdatePersonalFacet(dynamic gigyaModel, List<MappingField> mappingFields)
        //{
        //    if (mappingFields == null || !mappingFields.Any())
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        var facet = ContactProfileProvider.PersonalInfo;

        //        foreach (var mappingField in mappingFields)
        //        {
        //            switch (mappingField.CmsFieldName)
        //            {
        //                case nameof(IContactPersonalInfo.BirthDate):
        //                    facet.BirthDate = DynamicUtils.GetValue<DateTime?>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.FirstName):
        //                    facet.FirstName = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.Gender):
        //                    facet.Gender = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.JobTitle):
        //                    facet.JobTitle = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.MiddleName):
        //                    facet.MiddleName = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.Nickname):
        //                    facet.Nickname = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.Suffix):
        //                    facet.Suffix = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.Surname):
        //                    facet.Surname = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //                case nameof(IContactPersonalInfo.Title):
        //                    facet.Title = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
        //                    break;
        //            }
        //        }
        //    }
        //    catch (FacetNotAvailableException ex)
        //    {
        //        Log.Warn("The 'Personal' facet is not available.", ex);
        //    }
        //}
    }
}