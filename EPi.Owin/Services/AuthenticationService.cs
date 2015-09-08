using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Settings;
using Microsoft.AspNet.Identity;

namespace EPi.Owin.Services
{
    public static class AuthenticationService
    {
        public static void LogOff(OwinInputBase input)
        {
            var authenticationManager = input.OwinContext.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);

            var context = input.OwinContext.Get<HttpContextBase>(typeof(HttpContextBase).FullName);

            // session end removes the current user from online users
            var session = context.Session;
            if (session != null)
            {
                session.Clear();
                session.Abandon();
            }

            context.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            foreach (var cookie in context.Request.Cookies.AllKeys)
            {
                context.Request.Cookies.Remove(cookie);
            }
            foreach (var cookie in context.Response.Cookies.AllKeys)
            {
                context.Response.Cookies.Remove(cookie);
            }
        }
        
        public static void AuthorizeUser(ClaimsInput claimsInput)
        {
            var email = claimsInput.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var roles = claimsInput.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            
            var emailRolesInput = new EmailRolesInput
            {
                Email = email,
                Roles = roles,
                OwinContext = claimsInput.OwinContext
            };

            AuthorizeUser(emailRolesInput);
        }

        public static void AuthorizeUser(EmailRolesInput emailRoles)
        {
            var email = emailRoles.Email;
            var owinContext = emailRoles.OwinContext;

            var emailInput = new EmailInput { OwinContext = owinContext, Email = email };
            
            if (!ApplicationUserService.UserExists(emailInput))
            {
                ApplicationUserService.CreateUser(emailInput);
            }

            foreach (var role in emailRoles.Roles)
            {
                var roleInput = new RoleNameInput { OwinContext = owinContext, Role = role };
                if (!ApplicationRoleService.RoleExists(roleInput))
                {
                    ApplicationRoleService.CreateRole(roleInput);
                }

                var emailRoleInput = new EmailRoleInput { OwinContext = owinContext, Email = email, Role = role };
                ApplicationRoleService.AddUserToRole(emailRoleInput);
            }

            var emailAdminRoleInput = new EmailRoleInput { OwinContext = owinContext, Email = email, Role = SecurityAppSettings.AdminRole };
            if (SecurityAppSettings.AdminUsers.Contains(email) && !ApplicationUserService.IsUserInRole(emailAdminRoleInput))
            {
                var roleInput = new RoleNameInput { OwinContext = owinContext, Role = SecurityAppSettings.AdminRole };
                if (!ApplicationRoleService.RoleExists(roleInput))
                {
                    ApplicationRoleService.CreateRole(roleInput);
                }

                var emailRoleInput = new EmailRoleInput { OwinContext = owinContext, Email = email, Role = SecurityAppSettings.AdminRole };
                ApplicationRoleService.AddUserToRole(emailRoleInput);
            }
        }
    }
}
