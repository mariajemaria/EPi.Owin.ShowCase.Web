using System.Security.Claims;
using System.Threading.Tasks;
using EPi.Owin.Settings;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EPi.Owin.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string username)
            : base(username)
        {

        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicationDbContext()
                : base(SecurityAppSettings.DbNameForContext, throwIfV1Schema: false)
            {
            }

            public static ApplicationDbContext Create()
            {
                return new ApplicationDbContext();
            }
        }
    }
}