using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Appointment.Infrastructure.MiddleWare.RateLimitMiddleWare
{
  public class ClientPostBodyResolveContributor : IClientResolveContributor
    {
        private readonly string postBodyParamName;

        public ClientPostBodyResolveContributor(string postBodyParamName)
        {
            this.postBodyParamName = postBodyParamName;
        }

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            var userIdClaimFromToken = httpContext.Request.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ConstantRate.UserClaimType);
            if (userIdClaimFromToken == null)
            {
                return Task.FromResult("");
            }

            //TODO: (We don't use Identity server id) check if userid valid, resolve request to certain client when it contain userid,shouldn't hard code this, but can work for now 
            return Task.FromResult(ConstantRate.ClientId);
        }
    }
}