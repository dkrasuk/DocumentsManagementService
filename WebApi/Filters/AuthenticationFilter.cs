using BusinessLayer.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace WebApi.Filters
{
    /// <summary>
    /// Authentication Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticationFilter : AuthorizeAttribute
    {
        
        private IAccountService _accountService;

        /// <summary>
        /// 
        /// </summary>
        public AuthenticationFilter()
        {
            _accountService = ServiceLocator.Current.GetInstance<IAccountService>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsAuthenticate(filterContext))
            {
                throw new UnauthorizedAccessException();
            }
            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// Determines whether the specified filter context is authenticate.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>
        ///   <c>true</c> if the specified filter context is authenticate; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsAuthenticate(AuthorizationContext filterContext)
        {
            var login = filterContext.RequestContext.HttpContext.User.Identity.Name.ToLower().Replace("alfa\\", "");

            return (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated && _accountService.Authenticate(login));
        }


    }
}
