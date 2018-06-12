//using Sitecore.Analytics.Model.Entities;
//using Sitecore.Data.Items;
//using Sitecore.Gigya.Module.Helpers;
//using Sitecore.SecurityModel;
//using Sitecore.Shell.Applications.ContentEditor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using C = Sitecore.Gigya.Extensions.Abstractions.Analytics.Constants;

//namespace Sitecore.Gigya.Module.Fields
//{
//    public class FacetPropertyField : ValueLookupEx
//    {
//        private readonly ISitecoreContentHelper _sitecoreContentHelper;

//        public FacetPropertyField() : this(new SitecoreContentHelper())
//        {
//        }

//        public FacetPropertyField(ISitecoreContentHelper sitecoreContentHelper)
//        {
//            _sitecoreContentHelper = sitecoreContentHelper;
//        }

//        protected override Item[] GetItems(Item current)
//        {
//            using (new SecurityDisabler())
//            {
//                // find nearest parent that has a template of Sitecore xDB Facet Folder 
//                var folder = _sitecoreContentHelper.GetFacetFolderParent(current);

//                if (folder == null)
//                {
//                    return new Item[0];
//                }

//                var facetName = folder.Fields[Constants.Fields.FacetFolder.Name]?.Value;
//                if (string.IsNullOrEmpty(facetName))
//                {
//                    return new Item[0];
//                }

//                var type = GetFacet(facetName);
//                if (type == null)
//                {
//                    return new Item[0];
//                }

//                var properties = type
//                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                    .Where(i => i.CanWrite)
//                    .Select(Map)
//                    .ToArray();

//                return properties;
//            }
//        }

//        private Item Map(PropertyInfo propertyInfo)
//        {

//            var item = new Item(Data.ID.NewID, new Data.ItemData(, Sitecore.Context.ContentDatabase);
//            item.Name = propertyInfo.Name;
//            return item;
//        }

//        private Type GetFacet(string name)
//        {
//            switch (name)
//            {
//                case C.FacetKeys.Personal:
//                    return typeof(IContactPersonalInfo);
//                case C.FacetKeys.PhoneNumbers:
//                    return typeof(IContactPhoneNumbers);
//            }

//            return null;
//        }
//    }
//}