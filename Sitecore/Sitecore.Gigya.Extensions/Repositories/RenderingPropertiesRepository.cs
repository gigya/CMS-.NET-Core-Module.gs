namespace Sitecore.Gigya.Extensions.Repositories
{
    using System;
    using System.Linq;
    using SC = Sitecore;
    using Sitecore.Diagnostics;
    using Sitecore.Reflection;
    using Sitecore.Gigya.DependencyInjection;
    using System.Web;

    [Service(typeof(IRenderingPropertiesRepository))]
    public class RenderingPropertiesRepository : IRenderingPropertiesRepository
    {
        public T Get<T>(SC.Mvc.Presentation.Rendering rendering)
        {
            var obj = ReflectionUtil.CreateObject(typeof(T));
            var currentContext = rendering;
            var parameters = currentContext?.Properties["Parameters"];
            if (parameters == null)
                return (T)obj;

            parameters = this.FilterEmptyParametrs(parameters);
            try
            {
                ReflectionUtil.SetProperties(obj, parameters);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e, this);
            }

            return (T)obj;
        }

        protected virtual string FilterEmptyParametrs(string parameters)
        {
            var parametersList = parameters.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            var parameterString = string.Join("&", parametersList.Where(x => x.Contains("=")));
            if (!string.IsNullOrEmpty(parameterString))
            {
                parameterString = HttpUtility.UrlDecode(parameterString);
            }

            return parameterString;
        }
    }
}