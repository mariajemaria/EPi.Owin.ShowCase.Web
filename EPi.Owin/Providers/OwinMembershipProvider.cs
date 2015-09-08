using System;
using System.Web;
using System.Web.Security;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Services;

namespace EPi.Owin.Providers
{
    public class OwinMembershipProvider : MembershipProvider
    {
        public override string ApplicationName { get; set; }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
            out int totalRecords)
        {
            var usersByEmail = FindUsersByEmail(usernameToMatch, pageIndex, pageSize, out totalRecords);
            return usersByEmail;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var pagedInput = new PagedUsersInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchForEmail = (emailToMatch ?? "").Replace("%", "").ToLowerInvariant()
            };

            var allUsersDto = ApplicationUserService.GetUsers(pagedInput);

            totalRecords = allUsersDto.TotalNoOfUsers;
            var userCollection = new MembershipUserCollection();

            foreach (var user in allUsersDto.Users)
            {
                var membershipUser = new MembershipUser(Name, user.Email, user.Id, user.Email, null, null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
                userCollection.Add(membershipUser);
            }

            return userCollection;
        }
        
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var pagedInput = new PagedUsersInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var allUsersDto = ApplicationUserService.GetUsers(pagedInput);

            totalRecords = allUsersDto.TotalNoOfUsers;
            var userCollection = new MembershipUserCollection();

            foreach (var user in allUsersDto.Users)
            {
                var membershipUser = new MembershipUser(Name, user.Email, user.Id, user.Email, null, null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
                userCollection.Add(membershipUser);
            }

            return userCollection;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string email, bool userIsOnline)
        {
            var emailInput = new EmailInput
            {
                OwinContext = HttpContext.Current.GetOwinContext(),
                Email = email
            };

            var user = ApplicationUserService.GetUser(emailInput);

            if (user != null)
            {
                var membershipUser = new MembershipUser(Name, user.Email, user.Id, user.Email, null, null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
                return membershipUser;
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            // username and email are the same
            return email;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string passwordAnswer)
        {
            throw new NotSupportedException();
        }

        public override string ResetPassword(string username, string passwordAnswer)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string username)
        {
            throw new NotSupportedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotSupportedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 0; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotSupportedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotSupportedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 0; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotSupportedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }
    }
}