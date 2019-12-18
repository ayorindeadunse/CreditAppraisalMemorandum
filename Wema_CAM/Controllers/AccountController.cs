using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Wema_CAM.Filters;
using Wema_CAM.Models;
using System.DirectoryServices.ActiveDirectory;//call ActiveDir object
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Net.Mail;//call AccountManagement object

namespace Wema_CAM.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        string themaindescription = null;
        string userfunction = null;
        string path = null;
        string user = null;
        string pass = null;
        string mainuserregion = null;
        string mainuserbranch = null;
        string mainuserzone = null;
        long camuserid = 0;
        long camuserrid = 0;
        string newpasswordreset = null;
        string uusernamme = null;
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();

        }


        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                //  bool requireConfirmationToken = false;
                Wema_CAMEntities db = new Wema_CAMEntities();
                //  if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))

                // if((ModelState.IsValid) && Membership.ValidateUser(model.UserName, model.Password))

                path = ConfigurationManager.AppSettings["domain"].ToString();
                user = model.UserName;
                pass = model.Password;

                //string DisplayNames = UserPrincipal.Current.DisplayName;
                //string EmailAddress = UserPrincipal.Current.EmailAddress;
                //Session["DisplayNames"] = DisplayNames;
                //Session["EmailAddress"] = EmailAddress;

                // AuthenticateUser(user, pass);
               // if (ModelState.IsValid && AuthenticateUser(user, pass) == true)





                //  if (ModelState.IsValid && returnUserCredentials(user, pass) == true)
              if (ModelState.IsValid)
                {

                    string username = model.UserName.Trim();
                    CAM_User u = db.CAM_User.Single(i => i.Username == username);

                    if (u.Status != 1)
                    {
                        ViewBag.loginerror = "You user status is inactive. Kindly contact the Credit Risk Team.";
                    }

                    else
                    {
                        Session["UserFunction"] = u.Grade.ToString();
                        Session["BranchId"] = u.BranchId.ToString();
                        Session["Username"] = u.Username.ToString();
                        Session["EmailAddress"] = u.EmailAddress.ToString();
                        Session["CanApprove"] = u.CanApprove;
                        //Add the Region id to the session and use it to retrieve the actual region name so it can
                        //be saved as part of the CAM information.
                        long regionid = u.RegionId;
                        int zoneid = u.ZoneId;
                        string branchid = u.BranchId;


                        userfunction = Session["UserFunction"].ToString();

                        //Get user's full designation
                        var userfunctiondesc = from uu in db.UserFunction where uu.ShortDescription == userfunction select uu;
                        if (userfunctiondesc != null)
                        {
                            var description = userfunctiondesc.ToList();
                            foreach (var maindescription in description)
                            {
                                themaindescription = maindescription.Description;
                            }
                            Session["maindesignation"] = themaindescription;
                        }

                        var userregion = from ur in db.Region where ur.RegionId == regionid select ur;
                        if (userregion != null)
                        {
                            var regions = userregion.ToList();
                            foreach (var userregiondesc in regions)
                            {
                                mainuserregion = userregiondesc.RegionName;
                            }
                            Session["Region"] = mainuserregion;
                        }

                        var userbranch = from ub in db.Branch where ub.BranchId == branchid select ub;
                        if (userbranch != null)
                        {
                            var branches = userbranch.ToList();
                            foreach (var userbranchdesc in branches)
                            {
                                mainuserbranch = userbranchdesc.BranchName;
                            }
                            Session["BRANCH"] = mainuserbranch;
                        }

                        var userzone = from uz in db.Zone where uz.ZoneId == zoneid select uz;
                        if (userzone != null)
                        {
                            var zoness = userzone.ToList();
                            foreach (var userzonedesc in zoness)
                            {
                                mainuserzone = userzonedesc.ZoneName;
                            }
                            Session["ZONE"] = mainuserzone;
                        }
                        //  if (Membership.ValidateUser(model.UserName, model.Password))
                        //{
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        //   return RedirectToLocal(returnUrl);
                        return RedirectToLocal(returnUrl);
                    }
                }
                //}

            //    }

                //if (model.IsValid(model.UserName, model.Password))
                //{
                //    FormsAuthentication.SetAuthCookie(model.UserName, false);

                //    return RedirectToLocal(returnUrl);

                //}

                // If we got this far, something failed, redisplay form
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    return View(model);
                }
               
            }
            catch (Exception rcs)
            {
                Logging.WriteLog(rcs.Message + ":" + rcs.StackTrace);
                //Redirect user to error page.
                ViewBag.loginerror = "You are either supplying the wrong login credentials or you do not exist on the system. Please try again.";
               // return RedirectToAction("Exception", "CAMForm");
            }
            return View(model);
        }
            public bool AuthenticateUser(string _username, string _password)
        {

            string ad = ConfigurationManager.AppSettings["domainip"].ToString();//ConfigurationManager.AppSettings["AD"].ToString();

            string domain = ConfigurationManager.AppSettings["domaintext"].ToString();

            return CardUtil.Authenticate.AuthenticateUserAgainstAD(_username, _password, ad, domain);

        }

         private bool returnUserCredentials(string user,string pass)
         {
             //Encrypt password
            string encryptpass = FormsAuthentication.HashPasswordForStoringInConfigFile(pass, "SHA1");

            var usercredentials = from uc in db.CAM_User where uc.Username == user && uc.Password == encryptpass select uc;
            int resultcount = usercredentials.Count();

            if (resultcount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

         }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            WebSecurity.Logout();           
            return RedirectToAction("Index", "Home");
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
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
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
                 : message == ManageMessageId.LoginAgain ? "You need to Login again because your session has expired."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";

            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }



        [HttpGet]
        public ActionResult ChangePassword()
        {

            return View("_ChangePasswordPartial");

        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ResetPasswordModel model)
        {
            try
            {
                //string emailadd = resetppassword;
                string emailadd = model.EmailAddress;

                //Query database to see if e-mail address exists
                var emails = from em in db.CAM_User.Where(e => e.EmailAddress == emailadd) select em;

                int resultsetcount = emails.Count();
                if (resultsetcount == 1)
                {
                    //Send e-mail to user.
                    string passwordstring = Guid.NewGuid().ToString();
                    // defaultpassword = Membership.GeneratePassword(10,3);
                    newpasswordreset = passwordstring.Substring(10, 6);

                    string newpass = FormsAuthentication.HashPasswordForStoringInConfigFile(newpasswordreset, "SHA1");

                  
                foreach (var uservalues in emails)
                {
                    camuserrid = uservalues.id;
                    uusernamme = uservalues.Username;
                }

                CAM_User cu = db.CAM_User.Find(camuserrid);
                cu.Password = newpass;
                db.Entry(cu).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                    //Send e-mail
                resetmailUser(uusernamme, emailadd, newpass);

               ViewBag.NoEmailError = "Your password has been reset successfully.";

              
                }
                else
                {
                   ViewBag.NoEmailError = "Your e-mail address does not exist on the system.";
                }


                return Json(new { Result = "OK", Message = "Email successfully reset!" });
            }

            catch (Exception re)
            {
                Logging.WriteLog(re.Message + ":" + re.StackTrace);
                //Redirect user to error page.
              return  RedirectToAction("Exception", "CAMForm");
               // return Json(new { Result = "ERROR", Message = re.Message });
            }

          
           // return RedirectToAction("Login");
        }

        private void resetmailUser(string Username, string EmailAddress, string newpass)
        {
            //Remember to add the SMTP settings in the web.config file.
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(EmailAddress);
                mail.Subject = "Reset Password details for  " + Username; //" " + Session["lastname"].ToString();
                mail.Body = "Hi " + Username + "," + "\r\n\n" + "Your password has been reset to: " + newpasswordreset + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }
        }
        //
        // POST: /Account/Manage
        //[HttpPost]
        //public ActionResult Manage()
        //{

        //    return RedirectToAction("ChangePassword");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            //bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            //ViewBag.HasLocalPassword = hasLocalAccount;
            //ViewBag.ReturnUrl = Url.Action("Manage");

            string oldpass = model.OldPassword;
            string newpass = model.NewPassword;
            string userr = Session["Username"].ToString();
           
            if (userr != null)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                   // bool changePasswordSucceeded;
                    try
                    {
                        if (changepassword(oldpass,newpass) == true)
                        {
                            {
                                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }
                    catch (Exception e)
                    {
                       // changePasswordSucceeded = false;
                        Logging.WriteLog(e.Message + ":" + e.StackTrace);
                        //Redirect user to error page.
                        RedirectToAction("Exception", "CAMForm");
                    }

                    //if (changePasswordSucceeded)
                    //{
                    //    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    //}
                }

                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
            {
                //// User does not have a local password so remove any validation errors caused by a missing
                //// OldPassword field
                //ModelState state = ModelState["OldPassword"];
                //if (state != null)
                //{
                //    state.Errors.Clear();
                //}

                //if (ModelState.IsValid)
                //{
                //    try
                //    {
                //        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                //        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                //    }
                //    catch (Exception)
                //    {
                //        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                //    }
                //}

                return RedirectToAction("Manage", new { Message = ManageMessageId.LoginAgain });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private bool changepassword(string old,string newp)
        {

            // changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            //Encrypt old password

            string uusername = Session["Username"].ToString();
            string oldpass = FormsAuthentication.HashPasswordForStoringInConfigFile(old, "SHA1");
            string email = Session["EmailAddress"].ToString();

            var updatepassword = from np in db.CAM_User where np.Username == uusername && np.Password == oldpass select np;
            int resultcount = updatepassword.Count();

            if (resultcount > 0)
            {
                foreach (var uservalues in updatepassword)
                {
                    camuserid = uservalues.id;
                }

                //Change password, update database and send mail to user.
                //string passwordstring = Guid.NewGuid().ToString();
                
              //  newpassword = passwordstring.Substring(10, 6);

                string newpass = FormsAuthentication.HashPasswordForStoringInConfigFile(newp, "SHA1");

                CAM_User cu = db.CAM_User.Find(camuserid);
                cu.Password = newpass;
                db.Entry(cu).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                //Mail user

                mailUser(uusername, email, newp);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void mailUser(string Username, string EmailAddress, string newpass)
        {
            //Remember to add the SMTP settings in the web.config file.
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(EmailAddress);
                mail.Subject = "New Password details for  " + Username; //" " + Session["lastname"].ToString();
                mail.Body = "Hi " + Username + "," + "\r\n\n" + "Your new password is: " + newpass + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }
        }
        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            LoginAgain,         
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
