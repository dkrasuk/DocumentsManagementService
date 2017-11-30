using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class AllowCrossSiteAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = System.Web.HttpContext.Current;
            string origin = "";

            var origins = ConfigurationManager.AppSettings["CorsOriginsSettingKey"];
            Uri requestUrl = ctx.Request.UrlReferrer;
            if (origins != null && origins != "*")
            {
                
                string requestOrigin = "";

                if (requestUrl != null)
                {
                    requestOrigin = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                    if (origins.Split(';').Contains(requestOrigin))
                        origin = requestOrigin;
                }
            }
            else if (origins == "*")
            {
                origin = $"{requestUrl.Scheme}://{requestUrl.Authority}";
            }

            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", origin);
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");

            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.HttpContext.Response.Flush();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}