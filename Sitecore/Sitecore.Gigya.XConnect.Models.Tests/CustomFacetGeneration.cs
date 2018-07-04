using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sitecore.Gigya.XConnect.Models.Tests
{
    [TestClass]
    public class CustomFacetGeneration
    {
        [TestMethod]
        public void GenerateGigyaFacetBaseJson()
        {
            var model = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(GigyaXConnectFacetModel.Model);
            var path = Path.Combine("C:\\Temp\\Sitecore.Gigya.XConnect.Models.GigyaXConnectFacetModel, 1.0.json");
            File.WriteAllText(path, model);
        }
    }
}
