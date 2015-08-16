using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using WebApp;
using WebApp.Models;
using WebApp.Services;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApp.DomainModels.Customer;
using WebApp.Common;

namespace WebApp.Controllers
{
    [Authorize()]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ApplicationDbContext _applicationDbContext;

        //private static bool _databaseChecked;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> UserInfo() {
            EnsureDatabaseCreated(_applicationDbContext);
            ApplicationUser user = await GetCurrentUserAsync();
            return await GetUserInfo(user);
        }

        private async Task<IActionResult> GetUserInfo(ApplicationUser user) {
            try
            {
                if (user == null)
                {
                    return HttpNotFound();
                }

                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                if (user.MemberInfo == null)
                {
                    var menber = (from member in this._applicationDbContext.Members
                                  where member.MemberID.Equals(user.UserName)
                                  select new
                                  {
                                      ID = member.MemberID,
                                      Name = member.Name ?? "",
                                      Level = GetLevelDisplayName(member.Level),
                                      Role = role ?? "",
                                      RegisterDate = member.RegisterDate.ToString("yyyy'-'MM'-'dd"),
                                      NeedToChangePassword = user.ChangedPassword == false
                                  }).FirstOrDefault();
                    if (menber == null)
                    {
                        return HttpNotFound();
                    }
                    return new JsonResult(menber);
                }
                else
                {
                    return new JsonResult(new
                    {
                        ID = user.MemberInfo.MemberID,
                        Name = user.MemberInfo.Name ?? "",
                        Level = GetLevelDisplayName(user.MemberInfo.Level),
                        Role = role ?? "",
                        RegisterDate = user.MemberInfo.RegisterDate,
                        NeedToChangePassword = user.ChangedPassword == false
                    });
                }
            }
            catch (Exception e) {
                return new JsonResult(new
                {
                    Message = e.Message
                });
            }
        }

        private string GetLevelDisplayName(string level)
        {
            if ("Level1".Equals(level, StringComparison.InvariantCultureIgnoreCase)){
                return "高级会员";
            } else {
                return "普通会员";
            }
        }

        /*
        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }*/

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            EnsureDatabaseCreated(_applicationDbContext);
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserID, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //return RedirectToLocal(returnUrl);
                    ApplicationUser user = await _userManager.FindByNameAsync(model.UserID);
                     return await GetUserInfo(user);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay form
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        /*
        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }*/

        [HttpGet]
        public async Task<IActionResult> NextAccountID()
        {
            var newID  = await Task.Run<string>(() => {
                return IDGenerator.GetMemberIDGenerator(_applicationDbContext)
                                    .GetNext();
            } );
            return new JsonResult(new { NextAccountID = newID ?? ""});
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            EnsureDatabaseCreated(_applicationDbContext);
            if (ModelState.IsValid)
            {
                Member menberReference = null;
                string refID = model.ReferenceID;
                if (String.IsNullOrWhiteSpace(refID)) {
                    var adminUser = (await _userManager.GetUsersInRoleAsync("Administrator")).FirstOrDefault();
                    if (adminUser != null) {
                        refID = adminUser.UserName;
                    }
                }

                if (String.IsNullOrWhiteSpace(refID) == false) {
                    menberReference = (from m in this._applicationDbContext.Members
                                          where m.MemberID.Equals(refID) 
                                          select m 
                                       ).FirstOrDefault();
                }

                if (menberReference == null) {
                    return ErrorMessage.BadRequestJsonResult("ReferenceID is not exist.");
                }

                var member = new Member {
                    MemberID = model.AccountID,
                    Reference = menberReference,
                    Name = model.Name,
                    IDCard = model.CardID,
                    Address = model.Address,
                    Level = model.Level
                };

                var user = new ApplicationUser
                {
                    Id = model.AccountID,
                    UserName = model.AccountID,
                    MemberInfo = member,
                    PhoneNumber = model.Phone
                };

                this._applicationDbContext.Members.Add(member);

                var result = await _userManager.CreateAsync(user, ApplicationUser.IINT_PASSWORD);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Context.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction(nameof(HomeController.Index), "Home");
                    return new JsonResult("OK");
                }
                AddErrors(result);
            }
            
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            _signInManager.SignOut();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.ChangePasswordAsync(await GetCurrentUserAsync(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await GetCurrentUserAsync();
                    user.ChangedPassword = true;
                    var updateUserResult = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) {
                        return Json("OK");
                    }
                    else
                    {
                        return ErrorMessage.BadRequestJsonResult(updateUserResult.Errors);
                    }
                }
                else
                {
                    return ErrorMessage.BadRequestJsonResult(result.Errors);
                }
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        

        [HttpGet]
        public async Task<IActionResult> FindMember(FindMemberViewModel model) {
            if (ModelState.IsValid)
            {
                var result = from u in _applicationDbContext.Users
                             join m in _applicationDbContext.Members
                             on u.UserName equals m.MemberID
                             orderby m.RegisterDate descending
                             select new MemberInfoModel
                             {
                                 MemberID = m.MemberID,
                                 ReferenceID = m.ReferenceMemberID,
                                 Name = m.Name,
                                 IDCard = m.IDCard,
                                 Address = m.Address,
                                 Phone = u.PhoneNumber,
                                 Level = m.Level
                             };

                if (string.IsNullOrWhiteSpace(model.MemberID) == false) {
                    result = result.Where(
                        m =>  m.MemberID != null 
                              && m.MemberID.StartsWith(model.MemberID, StringComparison.InvariantCultureIgnoreCase));
                }
                
                if (string.IsNullOrWhiteSpace(model.ReferenceID) == false)
                {
                    result = result.Where(m => m.ReferenceID != null && m.ReferenceID.StartsWith(model.ReferenceID, 
                                                                        StringComparison.InvariantCultureIgnoreCase));
                }
                if (string.IsNullOrWhiteSpace(model.Name) == false)
                {
                    result = result.Where(m => m.Name != null && m.Name.IndexOf(model.Name) > -1);
                }

                if (string.IsNullOrWhiteSpace(model.IDCard) == false)
                {
                    result = result.Where(
                        m => m.IDCard != null && m.IDCard.StartsWith(model.IDCard, 
                                                     StringComparison.InvariantCultureIgnoreCase));
                }

                if (string.IsNullOrWhiteSpace(model.Phone) == false)
                {
                    result = result.Where(
                        m => m.Phone != null && m.Phone.StartsWith(model.Phone, 
                                                                    StringComparison.InvariantCultureIgnoreCase));
                }
       
               int size = await result.CountAsync();

              // paging
               result = result.Skip(model.page * model.PageSize).Take(model.PageSize);

              var items = result.ToList();

                return Json(new
                {
                    TotalSize = size,
                    Members = items
                });
           
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));

        }

        [HttpPost]
        public async Task<IActionResult> ModifyMember(MemberInfoModel model) {
            if (ModelState.IsValid)
            {
                var user = await _applicationDbContext.Users.
                               Where(u => u.UserName.Equals(model.MemberID, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefaultAsync();
                var member = await _applicationDbContext.Members
                               .Where(m => m.MemberID.Equals(model.MemberID, StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefaultAsync();

                if (user != null || member != null) {
                    if (user != null)
                    {
                        user.PhoneNumber = model.Phone;
                    }

                    if (member != null)
                    {
                        member.Address = model.Address;
                        member.Level = model.Level;
                    }

                    try
                    {
                          await _applicationDbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        return ErrorMessage.BadRequestJsonResult(e.Message);
                    }
                }
                return Json("OK");
            }
            return ErrorMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        /*
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            EnsureDatabaseCreated(_applicationDbContext);
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        
        
        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.ExternalPrincipal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.IsSignedIn())
            {
                return RedirectToAction(nameof(ManageController.Index),"Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Context.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                //return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }


        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        */

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError("", "Invalid code.");
                return View(model);
            }
        }

        #region Helpers

        // The following code creates the database and schema if they don't exist.
        // This is a temporary workaround since deploying database through EF migrations is
        // not yet supported in this release.
        // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
        // when publishing your application.
        private static void EnsureDatabaseCreated(ApplicationDbContext context)
        {
            /*
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                context.Database.AsRelational().ApplyMigrations();
            }*/
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
