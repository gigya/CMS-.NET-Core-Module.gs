using Gigya.Module.Core.Connector.Common;
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Gigya.Connector.Providers;
using Sitecore.Gigya.Connector.Services;
using Sitecore.Gigya.XConnect.Models;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Connector.Analytics.Conditions
{
    public class ContactGigyaFacetCondition<T> : OperatorCondition<T> where T : RuleContext
    {
        private static readonly ID _facetNameId = new ID(C.FacetKeys.FacetNamesId);

        public object FacetValue { get; set; }

        public ID FacetProperty { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Contact contact = Tracker.Current.Session.Contact;

            if (contact == null)
            {
                Log.Info(this.GetType() + ": contact is null", this);
                return false;
            }

            if (FacetProperty.IsNull)
            {
                Log.Info(this.GetType() + ": facet item is empty", this);
                return false;
            }

            var facetPropertyItem = ruleContext.Item.Database.GetItem(FacetProperty);
            if (facetPropertyItem == null)
            {
                Log.Info(this.GetType() + ": facet item is empty", this);
                return false;
            }

            var contentService = new SitecoreContentService();
            var propertyPathArr = contentService.FacetPath(facetPropertyItem, _facetNameId);

            if (propertyPathArr.Length < 2)
            {
                Log.Info(this.GetType() + ": facet path is empty", this);
                return false;
            }

            var provider = new ContactProfileProvider();

            var propertyQueue = new Queue<string>(propertyPathArr);
            string facetName = propertyQueue.Dequeue().ToString();
            var facet = GetGigyaFacet(provider, facetName);

            var memberName = propertyQueue.Dequeue().ToString();
            var conditionOperator = GetOperator();
            decimal? decimalRequiredValue = GetDecimalValue();
            return HasFacetValue(facet, memberName, decimalRequiredValue, conditionOperator);
        }

        private static GigyaXConnectFacet GetGigyaFacet(ContactProfileProvider provider, string facetName)
        {
            switch (facetName)
            {
                case C.FacetKeys.Gigya:
                    return provider.Gigya;
                case C.FacetKeys.GigyaPii:
                    return provider.GigyaPii;
            }

            return null;
        }

        private bool HasFacetValue(GigyaXConnectFacet facet, string memberName, decimal? requiredValue, ConditionOperator conditionOperator)
        {
            if (facet == null)
            {
                return false;
            }

            if (!facet.Entries.TryGetValue(memberName, out GigyaElement gigyaElement))
            {
                return false;
            }

            return CompareFacetValue(gigyaElement.Value, requiredValue, conditionOperator);
        }

        private decimal? GetDecimalValue()
        {
            decimal? decimalRequiredValue = null;
            if (FacetValue != null)
            {
                if (decimal.TryParse(FacetValue.ToString(), out decimal value))
                {
                    decimalRequiredValue = value;
                }
            }

            return decimalRequiredValue;
        }

        private bool CompareFacetValue(object propValue, decimal? requiredValue, ConditionOperator conditionOperator)
        {
            if (propValue == null)
            {
                return false;
            }

            switch (conditionOperator)
            {
                case ConditionOperator.Equal:
                    return propValue.Equals(FacetValue);
                case ConditionOperator.NotEqual:
                    return !propValue.Equals(FacetValue);
            }

            if (!decimal.TryParse(propValue.ToString(), out decimal value))
            {
                return false;
            }

            switch (conditionOperator)
            {
                case ConditionOperator.GreaterThanOrEqual:
                    return value >= requiredValue.Value;
                case ConditionOperator.GreaterThan:
                    return value > requiredValue.Value;
                case ConditionOperator.LessThanOrEqual:
                    return value <= requiredValue.Value;
                case ConditionOperator.LessThan:
                    return value < requiredValue.Value;
                default:
                    return false;
            }
        }
    }
}