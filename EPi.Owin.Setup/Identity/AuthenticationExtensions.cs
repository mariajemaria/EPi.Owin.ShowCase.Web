using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Services;
using EPi.Owin.Setup.Settings;
using log4net;
using log4net.Config;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Google;

namespace EPi.Owin.Setup.Identity
{
    public static class AuthenticationExtensions
    {
        public static Task OnAuthenticated(GoogleOAuth2AuthenticatedContext context)
        {
            var epiLog4NetConfigurationFilePath = HostingEnvironment.MapPath(@"\") + SecuritySetupAppSettings.EPiLog4NetFileName;
            XmlConfigurator.Configure(new FileInfo(epiLog4NetConfigurationFilePath));
            var logger = LogManager.GetLogger(typeof(AuthenticationExtensions));

            var userDetail = context.User;

            var email = context.Identity.FindFirstValue(ClaimTypes.Email);
            var domain = email.Substring(email.LastIndexOf("@", StringComparison.Ordinal));

            if (!SecuritySetupAppSettings.AllowedDomains.Contains(domain))
            {
                AuthenticationService.LogOff(new OwinInputBase { OwinContext = context.OwinContext });
                logger.ErrorFormat("User does not belong to the list of allowed domains: {0}; User: {1}", 
                    SecuritySetupAppSettings.AllowedDomains, userDetail);
            }
            else
            {
                try
                {
                    // adding this in order to authorize everyone (with allowed domains) with view rights
                    context.Identity.AddClaim(new Claim(ClaimTypes.Role, SecuritySetupAppSettings.AllUsersRole));
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("User could not be logged in. {0}", ex.Message);
                    AuthenticationService.LogOff(new OwinInputBase { OwinContext = context.OwinContext });
                    throw;
                }
            }

            return Task.FromResult(0);
        }
    }
}
