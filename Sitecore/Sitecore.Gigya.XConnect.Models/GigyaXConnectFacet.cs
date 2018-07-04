using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;

namespace Sitecore.Gigya.XConnect.Models
{
    public class GigyaXConnectFacet : Facet
    {
        public Dictionary<string, GigyaElement> Entries { get; set; }
    }

    [Serializable]
    public class GigyaElement
    {
        public string Value { get; set; }
    }

    public static class GigyaXConnectFacetModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("GigyaXConnectFacetModel", new XdbModelVersion(1, 0));
            builder.ReferenceModel(CollectionModel.Model);
            builder.RegisterType<GigyaXConnectFacet>(false);
            builder.RegisterType<GigyaElement>(false);
            builder.DefineFacet<Contact, GigyaFacet>(GigyaFacet.DefaultFacetKey);
            builder.DefineFacet<Contact, GigyaPiiFacet>(GigyaPiiFacet.DefaultFacetKey);
            return builder.BuildModel();
        }
    }
}