using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EPi.Owin.Identity;
using EPi.Owin.Models.Inputs;
using EPi.Owin.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace EPi.Owin.ShowCase.Web.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Action for Index view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var authorized = false;

            if (loginInfo != null)
            {
                // Sign in the user with this external login provider if the user already has a login
                var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

                // user and his login was created before
                if (result == SignInStatus.Success)
                {
                    AuthorizeUser(loginInfo);
                    authorized = true;
                }
                // either use or login was NOT created before
                else if (result == SignInStatus.Failure)
                {
                    var addLogin = true;
                    var user = await UserManager.FindByEmailAsync(loginInfo.Email);
                    if (user == null)
                    {
                        user = new ApplicationUser {UserName = loginInfo.Email, Email = loginInfo.Email};
                        var resultCreatedUser = await UserManager.CreateAsync(user);
                        if (!resultCreatedUser.Succeeded)
                        {
                            addLogin = false;
                        }
                    }

                    if (addLogin)
                    {
                        var resultLoginAdded = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                        if (resultLoginAdded.Succeeded)
                        {
                            user.SecurityStamp = DateTime.UtcNow.ToString("u");
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            AuthorizeUser(loginInfo);
                            authorized = true;
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Could not login user";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "SignInStatus - unexpected  " + result;
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "loginInfo is null";                

            }

            if (authorized)
            {
                return RedirectToLocal(returnUrl);
            }

            AuthenticationService.LogOff(new OwinInputBase { OwinContext = HttpContext.GetOwinContext() });

            ViewBag.ErrorMessage = "Authentication failure";
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }

        private void AuthorizeUser(ExternalLoginInfo loginInfo)
        {
            AuthenticationService.AuthorizeUser(new ClaimsInput
            {
                Claims = loginInfo.ExternalIdentity.Claims.ToList(),
                OwinContext = HttpContext.GetOwinContext()
            });
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);

            }
            return Redirect("/");
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }

}