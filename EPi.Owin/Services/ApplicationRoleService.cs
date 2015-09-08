using System;
using System.Collections.Generic;
using System.Linq;
using EPi.Owin.Identity;
using EPi.Owin.Models.Inputs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace EPi.Owin.Services
{
    public static class ApplicationRoleService
    {
        public static List<string> GetRoles(OwinInputBase inputContextBase)
        {
            var dbContext = inputContextBase.OwinContext.Get<ApplicationUser.ApplicationDbContext>();
            var roles = dbContext.Roles.Select(role => role.Name).ToList();
            return roles;
        }

        public static void CreateRole(RoleNameInput roleInput)
        {
            var dbContext = roleInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();
            dbContext.Roles.Add(new IdentityRole
            {
                Name = roleInput.Role
            });
            dbContext.SaveChanges();
        }

        public static bool RoleExists(RoleNameInput roleInput)
        {
            var thisRole = GetRole(roleInput);
            return thisRole != null;
        }

        public static bool DeleteRole(DeleteRoleNameInput roleInput)
        {
            var thisRole = GetRole(roleInput);
            if (thisRole != null)
            {
                if (roleInput.ThrowOnPopulatedRole)
                {
                    var usersInRole = GetUsersInRole(roleInput);
                    if (usersInRole != null && usersInRole.Any())
                    {
                        throw new Exception("Role is not empty");
                    }
                }

                var dbContext = roleInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();

                dbContext.Roles.Remove(thisRole);
                dbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public static List<string> GetUsersInRole(RoleNameInput roleInput)
        {
            var thisRole = GetRole(roleInput);

            if (thisRole != null)
            {
                var dbContext = roleInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();

                var users = dbContext.Users.AsEnumerable()
                         .Where(x => x.Roles.Select(y => y.RoleId).Contains(thisRole.Id))
                         .Select(r => r.UserName)
                         .ToList();

                return users;
            }

            return new List<string>(0);
        }

        public static IdentityRole GetRole(RoleNameInput roleInput)
        {
            var dbContext = roleInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();
            var thisRole = dbContext.Roles.FirstOrDefault(r => r.Name.Equals(roleInput.Role, StringComparison.CurrentCultureIgnoreCase));
            return thisRole;
        }


        public static List<string> GetRolesForUser(EmailInput usernameInput)
        {
            var user = ApplicationUserService.GetUser(usernameInput);
            if (user != null)
            {
                var userManager = usernameInput.OwinContext.GetUserManager<ApplicationUserManager>();
                var roles = userManager.GetRoles(user.Id);
                return roles.ToList();
            }

            return new List<string>(0);
        }

        public static void AddUserToRole(EmailRoleInput userRoleInput)
        {
            var user = ApplicationUserService.GetUser(new EmailInput { OwinContext = userRoleInput.OwinContext, Email = userRoleInput.Email });
            var userManager = userRoleInput.OwinContext.GetUserManager<ApplicationUserManager>();

            if (user != null && !userManager.IsInRole(user.Id, userRoleInput.Role))
            {
                userManager.AddToRole(user.Id, userRoleInput.Role);
            }
        }

        public static bool RemoveUserFromRole(EmailRoleInput userRoleInput)
        {
            var user = ApplicationUserService.GetUser(new EmailInput { OwinContext = userRoleInput.OwinContext, Email = userRoleInput.Email });
            if (user != null)
            {
                var userManager = userRoleInput.OwinContext.GetUserManager<ApplicationUserManager>();
                if (userManager.IsInRole(user.Id, userRoleInput.Role))
                {
                    userManager.RemoveFromRole(user.Id, userRoleInput.Role);

                    return true;
                }
            }

            return false;
        }
    }
}