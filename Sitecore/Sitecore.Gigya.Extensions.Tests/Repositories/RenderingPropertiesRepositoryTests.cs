using System;
using FluentAssertions;
using Sitecore.Gigya.Extensions.Repositories;
using Sitecore.Gigya.Testing.Attributes;
using Sitecore.Mvc.Presentation;
using Xunit;

namespace Sitecore.Gigya.Extensions.Tests.Repositories
{
    public class RenderingPropertiesRepositoryTests
    {
        [Theory]
        [AutoDbData]
        public void ShouldInitObjectProperties()
        {
            var rendering = new Rendering();
            rendering.Properties = new RenderingProperties(rendering) { ["Parameters"] = "property1=5&property2=10" };

            var repository = new RenderingPropertiesRepository();
            var resultObject = repository.Get<Model>(rendering);

            resultObject.Property1.ShouldBeEquivalentTo("5");
            resultObject.Property2.ShouldBeEquivalentTo("10");
        }

        [Theory]
        [AutoDbData]
        public void EncodedPropertiesAreSet()
        {
            var rendering = new Rendering();
            rendering.Properties = new RenderingProperties(rendering) { ["Parameters"] = "property1=Hello%20world" };

            var repository = new RenderingPropertiesRepository();
            var resultObject = repository.Get<Model>(rendering);
            resultObject.Property1.ShouldBeEquivalentTo("Hello world");
        }
    }

    public class Model
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}
