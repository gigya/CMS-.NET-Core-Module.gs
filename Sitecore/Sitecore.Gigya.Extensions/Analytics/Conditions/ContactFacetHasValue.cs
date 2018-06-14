using Sitecore.Analytics;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Gigya.Extensions.Analytics.Conditions
{
    public class ContactFacetHasValueCondition<T> : OperatorCondition<T> where T : RuleContext
    {
        public object FacetValue { get; set; }

        public string FacetPath { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Contact contact = Tracker.Current.Session.Contact;
            
            if (contact == null)
            {
                Log.Info(this.GetType() + ": contact is null", this);
                return false;
            }

            if (string.IsNullOrEmpty(FacetPath))
            {
                Log.Info(this.GetType() + ": facet path is empty", this);
                return false;
            }

            var inputPropertyToFind = FacetPath;

            string[] propertyPathArr = inputPropertyToFind.Split('.');
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

            IFacet facet = contact.Facets[facetName];
            if (facet == null)
            {
                Log.Info(string.Format("{0} : cannot find facet {1}", this.GetType(), facetName), this);
                return false;
            }

            var memberName = propertyQueue.Dequeue().ToString();
            if (!facet.Members.Contains(memberName))
            {
                Log.Info(string.Format("{0} : cannot find facet {1}", this.GetType(), facetName), this);
                return false;
            }

            var datalist = facet.Members[memberName];
            if (datalist == null)
            {
                Log.Info(string.Format("{0} : cannot find facet {1}", this.GetType(), facetName), this);
                return false;
            }

            var conditionOperator = GetOperator();
            decimal? requiredValue = null;
            if (FacetValue != null)
            {
                if (decimal.TryParse(FacetValue.ToString(), out decimal value))
                {
                    requiredValue = value;
                }
            }

            if (typeof(IModelAttributeMember).IsInstanceOfType(datalist))
            {
                var propValue = ((IModelAttributeMember)datalist).Value;
                return CompareFacetValue(propValue, requiredValue, conditionOperator);
            }
            if (typeof(IModelDictionaryMember).IsInstanceOfType(datalist))
            {
                var dictionaryMember = (IModelDictionaryMember)datalist;

                string elementName = propertyQueue.Dequeue().ToString();
                if (!dictionaryMember.Elements.Contains(elementName))
                {
                    Log.Info(string.Format("{0} : cannot find element {1}", this.GetType(), elementName), this);
                    return false;
                }

                IElement element = dictionaryMember.Elements[elementName];
                if (element == null)
                {
                    Log.Info(string.Format("{0} : cannot find element {1}", this.GetType(), elementName), this);
                    return false;
                }

                string propertyToFind = propertyQueue.Dequeue().ToString();
                if (!element.Members.Contains(propertyToFind))
                {
                    Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
                    return false;
                }

                var prop = element.Members[propertyToFind];
                if (prop == null)
                {
                    Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
                    return false;
                }

                var propValue = ((IModelAttributeMember)prop).Value;
                return CompareFacetValue(propValue, requiredValue, conditionOperator);
            }
            if (typeof(IModelCollectionMember).IsInstanceOfType(datalist))
            {
                var collectionMember = (IModelCollectionMember)datalist;
                var propertyToFind = propertyQueue.Dequeue().ToString();
                for (int i = 0; i < collectionMember.Elements.Count; i++)
                {
                    IElement element = collectionMember.Elements[i];
                    if (!element.Members.Contains(propertyToFind))
                    {
                        Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
                        return false;
                    }

                    var prop = element.Members[propertyToFind];
                    if (prop == null)
                    {
                        Log.Info(string.Format("{0} : cannot find property {1}", this.GetType(), propertyToFind), this);
                        return false;
                    }
                    var propValue = ((IModelAttributeMember)prop).Value;
                    if (CompareFacetValue(propValue, requiredValue, conditionOperator))
                    {
                        return true;
                    }
                }
            }

            return false;
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