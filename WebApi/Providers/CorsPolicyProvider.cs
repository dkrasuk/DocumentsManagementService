using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace WebApi.Providers
{
    /// <summary>
    /// Custom ocrs policy provider
    /// </summary>
    public class CorsPolicyProvider : ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true,
                PreflightMaxAge = 1728000,
            };

            _policy.Headers.Add("Content-Type");
            // Try and load allowed origins from web.config
            // If none are specified we'll allow all origins

            var origins = ConfigurationManager.AppSettings["CorsOriginsSettingKey"];

            if (origins != null)
            {
                foreach (var origin in origins.Split(';'))
                {
                    _policy.Origins.Add(origin);
                }
            }
            else
            {
                _policy.AllowAnyOrigin = true;
            }

            return _policy;
        }
    }
}