using System;
using System.Web;
using System.Web.Security;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Services;
using EPi.Owin.Settings;

namespace EPi.Owin.Providers
{
    public class OwinRoleProvider : RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override bool IsUserInRole(string username, string roleName)
        {
            var emailRoleInput = new EmailRoleInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Email = username,
                Role = roleName
            };

            var isUserInRole = ApplicationUserService.IsUserInRole(emailRoleInput);
            return isUserInRole;
        }

        public override string[] GetRolesForUser(string username)
        {
            var emailInput = new EmailInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Email = username
            };

            var roles = ApplicationRoleService.GetRolesForUser(emailInput);
            return roles.ToArray();
        }

        public override void CreateRole(string roleName)
        {
            CheckForAllowedRoles(roleName);

            var roleInput = new RoleNameInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Role = roleName
            };

            ApplicationRoleService.CreateRole(roleInput);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            CheckForAllowedRoles(roleName);

            var roleInput = new DeleteRoleNameInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Role = roleName,
                ThrowOnPopulatedRole = throwOnPopulatedRole
            };

            var deleted = ApplicationRoleService.DeleteRole(roleInput);
            return deleted;
        }

        public override bool RoleExists(string roleName)
        {
            var roleInput = new RoleNameInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Role = roleName
            };

            var role = ApplicationRoleService.GetRole(roleInput);
            return role != null;
        }


        public override string[] GetUsersInRole(string roleName)
        {
            var roleInput = new RoleNameInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Role = roleName
            };
            var usersInRole = ApplicationRoleService.GetUsersInRole(roleInput);
            return usersInRole.ToArray();
        }

        public override string[] GetAllRoles()
        {
            var owinBaseInput = new OwinInputBase
            {
                OwinContext = HttpContext.Current.GetOwinContext()
            };

            var roles = ApplicationRoleService.GetRoles(owinBaseInput);
            return roles.ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string emailToMatch)
        {
            throw new NotSupportedException();
        }

        public override void AddUsersToRoles(string[] emails, string[] roleNames)
        {
            foreach (var role in roleNames)
            {
                var canAddToRole = CheckForAllowedRoles(role, false);

                if (canAddToRole)
                {
                    foreach (var email in emails)
                    {
                        var emailRoleInput = new EmailRoleInput
                        {
                            Email = email,
                            Role = role,
                            OwinContext = HttpContext.Current.GetOwinContext()
                        };

                        ApplicationRoleService.AddUserToRole(emailRoleInput);
                    }
                }
            }
        }
        
        public override void RemoveUsersFromRoles(string[] emails, string[] roleNames)
        {
            foreach (var role in roleNames)
            {
                var canRemoveFromRole = CheckForAllowedRoles(role, false);

                if (canRemoveFromRole)
                {
                    foreach (var email in emails)
                    {
                        var emailRoleInput = new EmailRoleInput
                        {
                            Email = email,
                            Role = role,
                            OwinContext = HttpContext.Current.GetOwinContext()
                        };

                        ApplicationRoleService.RemoveUserFromRole(emailRoleInput);
                    }
                }
            }
        }

        // This method throws exceptions if role creation is not supported (AllowRolesCreation set to false)
        // or if the role does not begin with PrefixForAllowedRoles (PrefixForAllowedRoles can be left empty)
        private static bool CheckForAllowedRoles(string role, bool throwException = true)
        {
            if (!SecurityAppSettings.AllowRolesCreation)
            {
                if (throwException)
                {
                    throw new NotSupportedException("Roles creation not supported");
                }

                return false;
            }

            if (!string.IsNullOrEmpty(SecurityAppSettings.PrefixForAllowedRoles) &&
                !role.StartsWith(SecurityAppSettings.PrefixForAllowedRoles))
            {
                if (throwException)
                {
                    throw new NotSupportedException(string.Format("Cannot create a role, unless it begins with {0}",
                        SecurityAppSettings.PrefixForAllowedRoles));
                }

                return false;
            }

            return true;
        }
    }
}