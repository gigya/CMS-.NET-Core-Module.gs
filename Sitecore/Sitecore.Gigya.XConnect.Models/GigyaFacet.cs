﻿using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Gigya.XConnect.Models
{
    [FacetKey(DefaultFacetKey)]
    [Serializable]
    public class GigyaFacet : Facet
    {
        public const string DefaultFacetKey = "Gigya";

        public Dictionary<string, GigyaElement> Entries { get; set; }
    }

    [Serializable]
    public class GigyaElement
    {
        public string Value { get; set; }
    }

    public class GigyaModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("GigyaModel", new XdbModelVersion(1, 0));
            builder.ReferenceModel(CollectionModel.Model);
            builder.DefineFacet<Contact, GigyaFacet>(GigyaFacet.DefaultFacetKey);
            return builder.BuildModel();
        }
    }
}
