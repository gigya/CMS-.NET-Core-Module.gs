namespace Sitecore.Gigya.Testing.Attributes
{
  using Ploeh.AutoFixture.AutoNSubstitute;
  using Ploeh.AutoFixture.Kernel;
  using Ploeh.AutoFixture.Xunit2;
  using Sitecore.FakeDb.AutoFixture;
  using Sitecore.Gigya.Testing.Builders;
  using Sitecore.Gigya.Testing.Commands;

  public class AutoDbDataAttribute : AutoDataAttribute
  {
    public AutoDbDataAttribute()
    {
      this.Fixture.Customize(new AutoDbCustomization());
      this.Fixture.Customize(new AutoNSubstituteCustomization());
      this.Fixture.Customizations.Add(new Postprocessor(new ContentAttributeRelay(), new AddContentDbItemsCommand()));
      this.Fixture.Customizations.Insert(0, new RegisterViewToEngineBuilder());
      this.Fixture.Customizations.Add(new HtmlHelperBuilder());
      this.Fixture.Customizations.Add(new HttpContextBuilder());
    }
  }
}