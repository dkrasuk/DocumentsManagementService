using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace WebApi.Filters
{
    /// <summary>
    /// Kazna Exception Filter Attribute
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ExceptionFilterAttribute" />
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// On Exception Async
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            //var service = DependencyResolver.Current.GetService(typeof(ILogger)) as ILogger;
            var service = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILogger)) as ILogger;

            actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(
                                                 HttpStatusCode.InternalServerError, GetErrorMessage(actionExecutedContext.Exception));
            

            service.Error(actionExecutedContext.Exception.ToString());

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private static string GetErrorMessage(Exception ex)
        {
            var innerExceptionMessage = string.Empty;

            if (ex.InnerException != null && ex.InnerException.Message != string.Empty)
            {
                innerExceptionMessage = GetErrorMessage(ex.InnerException);
            }

            return $"{ex.Message}\n{innerExceptionMessage}";
        }
    }
}