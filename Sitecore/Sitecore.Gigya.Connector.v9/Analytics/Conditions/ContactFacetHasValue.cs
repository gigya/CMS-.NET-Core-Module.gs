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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

namespace Sitecore.Gigya.Connector.Analytics.Conditions
{
    public class ContactFacetHasValueCondition<T> : OperatorCondition<T> where T : RuleContext
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

            if (propertyPathArr.Length == 0)
            {
                Log.Info(this.GetType() + ": facet path is empty", this);
                return false;
            }

            var propertyQueue = new Queue<string>(propertyPathArr);
            string facetName = propertyQueue.Dequeue().ToString();
            if (!contact.Facets.ContainsKey(facetName))
            {
                Log.Info(string.Format("{0} : cannot find facet {1}", this.GetType(), facetName), this);
                return false;
            }

            var memberName = propertyQueue.Dequeue().ToString();

            var conditionOperator = GetOperator();
            decimal? decimalRequiredValue = GetDecimalValue();
            return HasFacetValue(facetName, memberName, decimalRequiredValue, conditionOperator);

            //return CompareFacetValue(propValue, decimalRequiredValue, conditionOperator);


            //if (typeof(IModelAttributeMember).IsInstanceOfType(datalist))
            //{
            //    var propValue = ((IModelAttributeMember)datalist).Value;
            //    return CompareFacetValue(propValue, decimalRequiredValue, conditionOperator);
            //}
            //if (typeof(IElementDictionary).IsInstanceOfType(datalist))
            //{
            //    var dictionaryMember = (IModelDictionaryMember)datalist;

            //    string elementName = propertyQueue.Dequeue().ToString();
            //    if (!dictionaryMember.Elements.Contains(elementName))
            //    {
            //        Log.Info(string.Format("{0} : cannot find element {1}", this.GetType(), elementName), this);
            //        return false;
            //    }

            //    IElement element = dictionaryMember.Elements[elementName];
            //    if (element == null)
            //    {
            //        Log.Info(string.Format("{0} : cannot find element {1}", this.GetType(), elementName), this);
            //        return false;
            //    }

            //    string propertyToFind = propertyQueue.Dequeue().ToString();
            //    if (!element.Members.Contains(propertyToFind))
            //    {
            //        Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
            //        return false;
            //    }

            //    var prop = element.Members[propertyToFind];
            //    if (prop == null)
            //    {
            //        Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
            //        return false;
            //    }

            //    var propValue = ((IModelAttributeMember)prop).Value;
            //    return CompareFacetValue(propValue, decimalRequiredValue, conditionOperator);
            //}
            //if (typeof(IModelCollectionMember).IsInstanceOfType(datalist))
            //{
            //    var collectionMember = (IModelCollectionMember)datalist;
            //    var propertyToFind = propertyQueue.Dequeue().ToString();
            //    for (int i = 0; i < collectionMember.Elements.Count; i++)
            //    {
            //        IElement element = collectionMember.Elements[i];
            //        if (!element.Members.Contains(propertyToFind))
            //        {
            //            Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
            //            return false;
            //        }

            //        var prop = element.Members[propertyToFind];
            //        if (prop == null)
            //        {
            //            Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
            //            return false;
            //        }
            //        var propValue = ((IModelAttributeMember)prop).Value;
            //        if (CompareFacetValue(propValue, decimalRequiredValue, conditionOperator))
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;
        }

        private bool HasFacetValue(string facetName, string memberName, decimal? requiredValue, ConditionOperator conditionOperator)
        {
            var legacyProvider = new LegacyContactProfileProvider();
            var provider = new ContactProfileProvider();

            switch (facetName)
            {
                case C.FacetKeys.Personal:
                    {
                        var facet = provider.PersonalInfo;
                        if (facet == null)
                        {
                            return false;
                        }

                        switch (memberName)
                        {
                            case nameof(IContactPersonalInfo.FirstName):
                                return CompareFacetValue(facet.FirstName, requiredValue, conditionOperator);
                        }
                    }
                    break;
                case C.FacetKeys.Addresses:
                    {
                        var facet = legacyProvider.Addresses;
                        if (facet == null)
                        {
                            return false;
                        }

                        foreach (var key in facet.Entries.Keys)
                        {
                            var address = facet.Entries[key];

                            // test.....
                            if (CompareFacetValue(address.PostalCode, requiredValue, conditionOperator))
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case C.FacetKeys.Gigya:
                    {
                        var facet = provider.Gigya;
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
            }

            return false;
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