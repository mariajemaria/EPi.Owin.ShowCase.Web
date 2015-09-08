using System;
using System.Collections.Generic;
using System.Linq;
using EPi.Owin.Identity;
using EPi.Owin.Models.Dtos;
using EPi.Owin.Models.Inputs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace EPi.Owin.Services
{
    public static class ApplicationUserService
    {
        public static ApplicationUser GetUser(EmailInput userInput)
        {
            if (string.IsNullOrEmpty(userInput.Email)) return null;

            var userManager = userInput.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = userManager.Users.FirstOrDefault(u => u.Email.Equals(userInput.Email, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public static void DeleteUser(EmailInput userInput)
        {
            var user = GetUser(userInput);
            if (user != null)
            {
                var userManager = userInput.OwinContext.GetUserManager<ApplicationUserManager>();
                userManager.Delete(user);
            }
        }

        public static bool IsUserInRole(EmailRoleInput userRoleInput)
        {
            if (string.IsNullOrEmpty(userRoleInput.Role)) return false;

            var user = GetUser(new EmailInput { OwinContext = userRoleInput.OwinContext, Email = userRoleInput.Email });
            if (user != null)
            {
                var userManager = userRoleInput.OwinContext.GetUserManager<ApplicationUserManager>();
                var isUserInRole = userManager.IsInRole(user.Id, userRoleInput.Role);
                return isUserInRole;
            }

            return false;
        }

        public static bool UserExists(EmailInput usernameInput)
        {
            var user = GetUser(usernameInput);
            return user != null;
        }

        public static void CreateUser(EmailInput usernameInput)
        {
            var dbContext = usernameInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();

            dbContext.Users.Add(new ApplicationUser
            {
                Email = usernameInput.Email,
                UserName = usernameInput.Email
            });

            dbContext.SaveChanges();
        }

        public static AllUsersDto GetUsers(PagedUsersInput pagedUsersInput)
        {
            var dbContext = pagedUsersInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();

            // either search by email or get all
            var allUsers = !string.IsNullOrEmpty(pagedUsersInput.SearchForEmail) ?
                dbContext.Users.Where(u => u.Email.ToLower().Contains(pagedUsersInput.SearchForEmail)) :
                dbContext.Users;

            var totalRecords = allUsers.Count();

            var allUsersDto = new AllUsersDto
            {
                TotalNoOfUsers = totalRecords,
                Users = allUsers
                        .OrderBy(u => u.Email)
                        .Skip(pagedUsersInput.PageIndex * pagedUsersInput.PageSize)
                        .Take(pagedUsersInput.PageSize)
                        .ToList()
            };

            return allUsersDto;
        }

        public static List<string> GetDeletedUsersEmail(PagedUsersInput pagedUsersInput)
        {
            var dbContext = pagedUsersInput.OwinContext.Get<ApplicationUser.ApplicationDbContext>();

            var usersToDelete = dbContext.Users
               .Where(p => !pagedUsersInput.EmailsToCompareForDeletion.Any(v => v.Contains(p.Email)))
               .Select(p => p.Email)
               .ToList();

            return usersToDelete;
        }
    }
}