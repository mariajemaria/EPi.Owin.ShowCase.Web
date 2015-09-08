using System;
using EPi.Owin.Identity;
using EPi.Owin.Setup.Identity;
using EPi.Owin.Setup.Settings;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace EPi.Owin.Setup.Identity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationUser.ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval: TimeSpan.FromMinutes(SecuritySetupAppSettings.ValidateIdentityInterval),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                ExpireTimeSpan = TimeSpan.FromMinutes(SecuritySetupAppSettings.LogoutIdleTimeSpan),
                LoginPath = new PathString("/Account")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = SecuritySetupAppSettings.GoogleAuthenticationClientId,
                ClientSecret = SecuritySetupAppSettings.GoogleAuthenticationClientSecret,

                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context => AuthenticationExtensions.OnAuthenticated(context)
                }
            });
        }
    }
}