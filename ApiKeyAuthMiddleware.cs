using busylight_server.ApiKeyAuthMiddleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiKeyAuthMiddlewareExtensions
    {
        public static void AddApiKeyAuthentication(this IServiceCollection services, Action<ApiKeyAuthOptions> configOptions)
        {
            services.AddAuthentication(options =>
            {
                // the scheme name has to match the value we're going to use in AuthenticationBuilder.AddScheme(...)
                options.DefaultAuthenticateScheme = "ApiKey Scheme";
                options.DefaultChallengeScheme = "ApiKey Scheme";
            }).AddScheme<ApiKeyAuthOptions, ApiKeyAuthHandler>("ApiKey Scheme", "ApiKey Auth", configOptions);
        }
    }
}

namespace busylight_server.ApiKeyAuthMiddleware
{
    public class ApiKeyAuthOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Name of the key to look for.
        /// Will look in both Request.Headers and Request.Query for the key.
        /// Default Value is "ApiKey"
        /// </summary>
        public string KeyName = "ApiKey";

        /// <summary>
        /// Your super secret value that should be in "KeyName"
        /// If you need to specify multiple keys then do it as comma seperated.
        /// </summary>
        public string[] ApiKeys { get; set; }
    }

    internal class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOptions>
    {
        public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            // store custom services here...
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var key = string.Empty;
            var identity = new ClaimsIdentity(Options.KeyName);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null, Options.KeyName);

            if (Options.ApiKeys.Length == 0)
                return Task.FromResult(AuthenticateResult.Success(ticket));


            if (Request.Headers[Options.KeyName].Any())
                key = Request.Headers[Options.KeyName].FirstOrDefault();

            if (Request.Query.ContainsKey(Options.KeyName))
                key = Request.Query[Options.KeyName];




            if (Options.ApiKeys.Contains(key))
                return Task.FromResult(AuthenticateResult.Success(ticket));


            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }

}