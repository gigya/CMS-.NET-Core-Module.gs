using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Gigya.XConnect.Models.Tests
{
    [TestClass]
    public class CustomFacetGeneration
    {
        [TestMethod]
        public void GenerateJson()
        {
            var model = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(GigyaModel.Model);
            var path = Path.Combine("C:\\Temp", GigyaModel.Model.FullName + ".json");
            File.WriteAllText(path, model);
        }
    }
}
