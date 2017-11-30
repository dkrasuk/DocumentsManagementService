using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Filters
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionsAttribute : ActionFilterAttribute
    {
        private readonly Permissions required;
        private IAccountService accountService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        [InjectionConstructor]
        public PermissionsAttribute(Permissions required)
        {
            this.required = required;
            this.accountService = ServiceLocator.Current.GetInstance<IAccountService>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var user = actionContext.RequestContext.HttpContext.User.Identity.Name.ToLower().Replace("alfa\\", "");

            if (user == null)
                throw new UnauthorizedAccessException();

            var s = required.ToString();
            if (!accountService.Authorize(user, s))
                throw new AuthenticationException("You do not have the necessary permission to perform this action");
        }
    }
}