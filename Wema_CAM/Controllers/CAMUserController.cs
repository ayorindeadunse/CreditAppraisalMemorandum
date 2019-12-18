using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;
using System.Web.Security;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using WebMatrix.WebData;




namespace Wema_CAM.Controllers
{
    public class CAMUserController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();
        string defaultpassword = null;

        #region JSON
        public JsonResult GetBranchList()
        {
            try
            {
                var branches = db.Branch.Select(
                    c => new { DisplayText = c.BranchName, Value = c.BranchId });
                return Json(new { Result = "OK", Options = branches });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult GetFunctionList()
        {
            try
            {
                var functions = db.UserFunction.Select(
                    f => new { DisplayText = f.Description, Value = f.ShortDescription });
                return Json(new { Result = "OK", Options = functions });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
        public JsonResult GetStatusList()
        {
            try
            {
                var userstate = db.Status.Select(
                    s => new { DisplayText = s.Description, Value = s.StatusID});
                return Json(new { Result = "OK", Options = userstate });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult GetRoleList()
        {
            try
            {
            //    string[] roleList = Roles.GetAllRoles();


            //    var list = from r in roleList
            //               select new { DisplayText = r, Value = r };
            //    return Json(new { Result = "OK", Options = list });

                return null;
               
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult GetCAMUserList()
        {
            try
            {
                List<CAM_User> CAMUserList = db.CAM_User.ToList();
                int recordCount = CAMUserList.Count;
                return Json(new { Result = "OK", Records = CAMUserList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddCAMUser(CAM_User CAMUser)
        {
            //Suspend the use of the Membership provider and instead create normal Forms Authentication.

            try
            {
                string passwordstring = Guid.NewGuid().ToString();
                // defaultpassword = Membership.GeneratePassword(10,3);
                defaultpassword = passwordstring.Substring(10,6);

                //Encrypt the password
                //FormsAuthentication.HashPasswordForStoringInConfigFile(defaultpassword, "SHA1");

                //Store user information in sql server.

                CAM_User cuser = new CAM_User();

                cuser.Username = CAMUser.Username;
                cuser.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(defaultpassword, "SHA1");
                cuser.EmailAddress = CAMUser.EmailAddress;
                cuser.Role = CAMUser.Role;

                var branch = db.Branch.Where(b => b.BranchId == CAMUser.BranchId).SingleOrDefault();
                
                cuser.RegionId = branch.RegionId;
                cuser.ZoneId = branch.ZoneId;

                cuser.BranchId = CAMUser.BranchId;
                cuser.Grade = CAMUser.Grade;
                cuser.Status = CAMUser.Status;

                if (CAMUser.CanApprove != null)
                {
                    cuser.CanApprove = CAMUser.CanApprove;
                }

                cuser.CreationDate = DateTime.Now;
                cuser.DateLastUpdated = DateTime.Now;
                cuser.UpdateUserId = CAMUser.Username;

                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added CAMUser " + CAMUser.Username, UserId = "Admin" };
                //   Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added CAMUser " + CAMUser.Username, UserId = "test Credit Risk Team", Designation=Session["maindesignation"].ToString() };
                // db.Audit.Add(userAudit);
                db.CAM_User.Add(cuser);
                db.SaveChanges();


             
             // MembershipUser newUser = Membership.CreateUser(CAMUser.Username, defaultpassword, CAMUser.EmailAddress);

             //   if (!(newUser == null))
             //   {
             //       Roles.AddUserToRole(CAMUser.Username, CAMUser.Role);
             //   }

             //   //profile user 
             //   CAMUser.CreationDate = DateTime.Now;
             //   CAMUser.DateLastUpdated = DateTime.Now;
             //   CAMUser.UpdateUserId = CAMUser.Username;
               

             //   var branch = db.Branch.Where(b => b.BranchId == CAMUser.BranchId).SingleOrDefault();
             //   CAMUser.RegionId = branch.RegionId;
             //   CAMUser.ZoneId = branch.ZoneId;
             //   if (!ModelState.IsValid)
             //   {
             //       return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
             //   }

             //   //Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added CAMUser " + CAMUser.Username, UserId = User.Identity.Name };
             ////   Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added CAMUser " + CAMUser.Username, UserId = "test Credit Risk Team", Designation=Session["maindesignation"].ToString() };
             //  // db.Audit.Add(userAudit);
             //   db.CAM_User.Add(CAMUser);
             //   db.SaveChanges();

                //set user profile in asp.net membership provider
             
              mailUser(CAMUser.Username, CAMUser.EmailAddress);
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = CAMUser });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }
        [HttpPost]
        public JsonResult EditCAMUser(CAM_User CAMUser)
        {
            try
            {
                CAMUser.CreationDate = DateTime.Now;
                CAMUser.DateLastUpdated = DateTime.Now;
                CAMUser.UpdateUserId = CAMUser.Username;

                var branch = db.Branch.Where(b => b.BranchId == CAMUser.BranchId).SingleOrDefault();
                CAMUser.RegionId = branch.RegionId;
                CAMUser.ZoneId = branch.ZoneId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
               // Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated CAMUser " + CAMUser.Username, UserId = User.Identity.Name };
              //  Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated CAMUser " + CAMUser.Username, UserId = "test Credit Risk Team", Designation=Session["maindesignation"].ToString() };

                //db.Audit.Add(userAudit);
                db.Entry(CAMUser).State = EntityState.Modified;
                db.SaveChanges();

                //string[] userRole = Roles.GetRolesForUser(CAMUser.Username);
                //Roles.RemoveUserFromRole(CAMUser.Username, userRole[0]);
                //Roles.AddUserToRole(CAMUser.Username, CAMUser.Role);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteCAMUser(int id)
        {
            try
            {
                Thread.Sleep(50);
                CAM_User CAMUser = db.CAM_User.Find(id);
                db.CAM_User.Remove(CAMUser);
               // Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted CAMUser " + CAMUser.Username, UserId = User.Identity.Name};
              //  Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted CAMUser " + CAMUser.Username, UserId = "test Credit Risk Team" ,Designation=Session["maindesignation"].ToString()};

               // db.Audit.Add(userAudit);
                db.SaveChanges();

                //clear membership details
                //string[] userRole = Roles.GetRolesForUser(CAMUser.Username);
                //Roles.RemoveUserFromRole(CAMUser.Username, userRole[0]);
                //Membership.DeleteUser(CAMUser.Username);


                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        #endregion

        //
        // GET: /CAMUser/

        public ActionResult Index()
        {
            return View(db.CAM_User.ToList());
        }

        //
        // GET: /CAMUser/Details/5

        public ActionResult Details(int id = 0)
        {
            CAM_User cam_user = db.CAM_User.Find(id);
            if (cam_user == null)
            {
                return HttpNotFound();
            }
            return View(cam_user);
        }

        //
        // GET: /CAMUser/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CAMUser/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CAM_User cam_user)
        {
            if (ModelState.IsValid)
            {
                db.CAM_User.Add(cam_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cam_user);
        }

        //
        // GET: /CAMUser/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CAM_User cam_user = db.CAM_User.Find(id);
            if (cam_user == null)
            {
                return HttpNotFound();
            }
            return View(cam_user);
        }

        //
        // POST: /CAMUser/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CAM_User cam_user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cam_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cam_user);
        }

        //
        // GET: /CAMUser/Delete/5

        public ActionResult Delete(int id = 0)
        {
            CAM_User cam_user = db.CAM_User.Find(id);
            if (cam_user == null)
            {
                return HttpNotFound();
            }
            return View(cam_user);
        }

        //
        // POST: /CAMUser/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CAM_User cam_user = db.CAM_User.Find(id);
            db.CAM_User.Remove(cam_user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        //Mail method

        private void mailUser(string Username,  string EmailAddress)
        {
            //Remember to add the SMTP settings in the web.config file.
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(EmailAddress);
                mail.Subject = "Password details for  " + Username; //" " + Session["lastname"].ToString();
               // mail.Body = "Hi " + Username + "," + "\r\n\n" + "Your password is: " + defaultpassword + "\r\n\n" + "Ensure you change your password when next you log in." + "\r\n\n" + "The Credit Risk Management Team.";
                mail.Body = "Hi " + Username + "," + "\r\n\n" + "You have been profiled on the Credit Appraisal Memorandum(CAM) application. You are required to use your login credentials on Active Directory to have access to the service. Warm Regards," + "\r\n\n" + "The Credit Risk Management Team.";
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
    }
}