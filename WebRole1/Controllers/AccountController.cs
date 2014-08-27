using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Facebook;
using System.Net;
using Owin.Security.Providers.Yahoo;
using System.Net.Mail;
using System.Configuration;
using WebRole1.Models;


namespace WebRole1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //var result = AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            //if (result != null)
            //{
            //    if (result.Result != null)
            //    {
            //        var name = result.Result.Identity.Name;
            //        Session["name"] = name;
            //       return RedirectToAction("SuccessLogin", "Account");
            //    }
            //    else
            //    {
            //        return View();
            //    }
            //}
            //else
            //{
            //    return View();
            //}
           // var result1 = AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

          //  var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            //if (result == null || result.Result.Identity == null)
            //{
            //    return RedirectToAction("Login");
            //}

            //var idClaim = result.Result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            //if (idClaim == null)
            //{
            //    return RedirectToAction("Login");
            //}

            //var login = new UserLoginInfo(idClaim.Issuer, idClaim.Value);
            //var name = result.Result.Identity.Name == null ? "" : result.Result.Identity.Name.Replace(" ", "");

            //// Sign in the user with this external login provider if the user already has a login
            //var user =  UserManager.FindAsync(login);
            //if (user != null)
            //{
            //    //await SignInAsync(user, isPersistent: false);
            //    //return RedirectToLocal(returnUrl);
            //    // await FbAuthenticationToken(user);
            //    // SignInAsync(user, isPersistent: false);
            //   // return RedirectToLocal(returnUrl);
            //}
            //else
            //{
            //    // If the user does not have an account, then prompt the user to create an account
            //    ViewBag.ReturnUrl = returnUrl;
            //    ViewBag.LoginProvider = login.LoginProvider;
            //    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = name });
            //}




           // ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    Session["name"] = model.UserName;
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                   ViewBag.InvalidUser = "Invalid username or password.";
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: /Account/Reset Password
        [HttpGet]
         [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }
         // POST: /Account/Reset Password
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
         {
             if (ModelState.IsValid)
             {
                 var user = await UserManager.FindByNameAsync(model.UserName);
                 if (user == null )//|| !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                 {
                     // Don't reveal that the user does not exist or is not confirmed
                     ViewBag.UserNotExist = "User doesn't exist! Please fill correct User.";
                     return View("ResetPassword");
                 }

                 // var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);, code = code
                 var callbackUrl = Url.Action("ChangePassword", "Account",
                 new { UserId = user.Id }, protocol: Request.Url.Scheme);
               //  await UserManager.SendEmailAsync(user.Id, "Reset Password",
               //  "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                bool status = EmailService.SendMailMessage("Reset Password", "Please reset your password: <a href=\"" + callbackUrl + "\">click here</a>",user.Email);
                if (status == true)
                {
                    ViewBag.SendEmail = "Please check your email for reset password!";
                }

                 return View();
             }

             // If we got this far, something failed, redisplay form
             return View(model);
         }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            string UserId = Request.QueryString["UserId"];
            TempData["UserId"] = UserId;
            return View();
        }
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var UserId = TempData["UserId"].ToString();
                //var result = await UserManager.ResetPasswordAsync((UserId, "", model.NewPassword);
                 UserManager.RemovePassword(UserId);

                var result =  UserManager.AddPassword(UserId, model.NewPassword);
                //var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
               // var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.ChangePassword = "Your password has been changed successfully.";
                    //await SignInAsync(user, isPersistent: false);
                    return View();
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
       
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName,Email=model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   
                    ViewBag.CreatedUser = "You have been successfully registered.";
                   // await SignInAsync(user, isPersistent: false);
                  //  return RedirectToAction("Index", "Home");
                }
                else
                {
                  //  IEnumerable<string> msg = result.Errors;
                   // ViewBag.CreatedUser = msg;
                    foreach (var error in result.Errors)
                    {
                        ViewBag.ErrorMsg = error;
                    }
                    //AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            ControllerContext.HttpContext.Session.RemoveAll();
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (result == null || result.Identity == null)
            {
                return RedirectToAction("Login");
            }

            var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return RedirectToAction("Login");
            }

            var login = new UserLoginInfo(idClaim.Issuer, idClaim.Value);
            var name = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", "");

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(login);
            if (user != null)
            {
               
                //await SignInAsync(user, isPersistent: false);
                //return RedirectToLocal(returnUrl);
               // await FbAuthenticationToken(user);
                await SignInAsync(user, isPersistent: false);
                Session["name"] = user.UserName;
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = name });
            }
           
        }

        //
        // POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //}

        ////
        //// GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        var FbUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        //await FbAuthenticationToken(FbUser);
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });


        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Manage");
            //}

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                       
                       // await FbAuthenticationToken(user);
                        await SignInAsync(user, isPersistent: false);
                        Session["name"] = model.UserName;
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
       
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
           
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            Session["name"] = null;
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //public async Task<ActionResult> FacebookFriends()
        //{
        //    var UserClaim = await UserManager.GetClaimsAsync(User.Identity.GetUserId());
 
        //    var token = UserClaim.FirstOrDefault(fb => fb.Type == "FacebookAccessToken").Value;
 
        //    var FbClient = new Facebook.FacebookClient(token);
 
        //    dynamic FacebookFriends = FbClient.Get("/me/friends");
        //    var FbFriends=new List<FacebookViewModel>();
        //    foreach (dynamic MyFriend in FacebookFriends.data)
        //    {
        //        FbFriends.Add(new FacebookViewModel()
        //        {
        //            Name=MyFriend.name,
        //            Image= @"https://graph.facebook.com/" + MyFriend.id + "/picture?type=large"
        //        });
        //    }
 
        //   return View(FbFriends);
        //}
        //private async Task FbAuthenticationToken(ApplicationUser User)
        //{
        //    var claims = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
        //    if (claims != null)
        //    {
        //        var getclaim = await UserManager.GetClaimsAsync(User.Id);
        //        var FbToken = claims.FindAll("FacebookAccessToken").First();
        //        if (getclaim.Count() <= 0)
        //        {
        //            await UserManager.AddClaimAsync(User.Id, FbToken);
        //        }
        //    }
        //}
        public ActionResult SuccessLogin()
        {
            return View();
        }
        private class ChallengeResult : HttpUnauthorizedResult
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}