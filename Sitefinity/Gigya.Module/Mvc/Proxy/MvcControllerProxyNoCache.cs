using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI.PublicControls;

namespace Gigya.Module.Mvc.Proxy
{
    public class MvcControllerProxyNoCache : MvcControllerProxy
    {
        private bool IsPostBack
        {
            get { return HttpContext.Current.Request.HttpMethod == WebRequestMethods.Http.Post; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (IsPostBack || SystemManager.IsDesignMode)
            {
                base.OnPreRender(e);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (IsPostBack || SystemManager.IsDesignMode)
            {
                base.Render(writer);
            }
            else
            {
                var parameters = new Dictionary<string, string>
                {
                    { "ControllerName", this.ControllerName },
                    { "SerializedSettings", this.SerializedSettings  }
                };
                var renderMarkup = new CacheSubstitutionWrapper.RenderMarkupDelegate(MvcControllerProxyNoCache.RenderStatic);
                var wrapper = new CacheSubstitutionWrapper(parameters, renderMarkup);

                wrapper.RegisterPostCacheCallBack(HttpContext.Current);
            }
        }

        public static string RenderStatic(Dictionary<string, string> parameters)
        {
            var controllerActionInvoker = ObjectFactory.Resolve<IControllerActionInvoker>();
            var proxyControl = new MvcControllerProxy
            {
                ControllerName = parameters["ControllerName"],
                SerializedSettings = parameters["SerializedSettings"]
            };

            string output;
            if (controllerActionInvoker.TryInvokeAction(proxyControl, out output))
                return output;

            return string.Empty;
        }
    }
}