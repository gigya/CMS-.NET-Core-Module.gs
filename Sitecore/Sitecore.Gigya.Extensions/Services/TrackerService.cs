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
using System.Web;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Extensions.Services
{
    [Service(typeof(ITrackerService))]
    public class TrackerService : ITrackerService
    {
        public bool IsActive => Tracker.Current != null && Tracker.Current.IsActive;

        public void IdentifyContact(string identifier)
        {
            try
            {
                if (IsActive)
                {
                    Tracker.Current.Session.Identify(identifier);
                }
            }
            catch (ItemNotFoundException ex)
            {
                //Error can happen if previous user profile has been deleted
                Log.Error($"Could not identify the user '{identifier}'", ex, this);
            }
        }

        public void UpdateFacets(dynamic gigyaModel, List<MappingFieldGroup> mappingFields)
        {
            foreach (var group in mappingFields)
            {
                UpdateFacet(gigyaModel, group);
            }
        }

        private void UpdateFacet(dynamic gigyaModel, MappingFieldGroup group)
        {
            switch (group.FacetName)
            {
                case C.FacetKeys.PersonalFacetKey:
                    UpdatePersonalFacet(gigyaModel, group.Fields);
                    break;
                case C.FacetKeys.AddressesFacetKey:
                    //UpdatePersonalFacet(gigyaModel, group.Fields);
                    break;
            }
        }

        private void UpdatePersonalFacet(dynamic gigyaModel, List<MappingField> mappingFields)
        {
            if (mappingFields == null || !mappingFields.Any())
            {
                return;
            }

            try
            {
                var contact = Tracker.Current.Contact;
                var facet = contact.GetFacet<IContactPersonalInfo>(C.FacetKeys.PersonalFacetKey);
                
                foreach (var mappingField in mappingFields)
                {
                    switch (mappingField.CmsFieldName)
                    {
                        case nameof(IContactPersonalInfo.BirthDate):
                            facet.BirthDate = DynamicUtils.GetValue<DateTime?>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.FirstName):
                            facet.FirstName = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.Gender):
                            facet.Gender = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.JobTitle):
                            facet.JobTitle = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.MiddleName):
                            facet.MiddleName = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.Nickname):
                            facet.Nickname = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.Suffix):
                            facet.Suffix = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.Surname):
                            facet.Surname = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
                        case nameof(IContactPersonalInfo.Title):
                            facet.Title = DynamicUtils.GetValue<string>(gigyaModel, mappingField.GigyaFieldName);
                            break;
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