using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;


namespace Wema_CAM.Controllers
{

    public class CAMTemplateController : Controller
    {
        string useremailaddress = null;
        string originatinguser = null;
        string crrtext = null;
        string crrtext1 = null;
        long appid = 0;
        long cid = 0;
        

        private Wema_CAMEntities db = new Wema_CAMEntities();

        #region JSON

        public JsonResult GetApplicantType()
        {
            try
            {
                var applicanttype = db.ApplicantType.Select(
                    c => new { DisplayText = c.descritpion, Value = c.id });
                return Json(new { Result = "OK", Options = applicanttype });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetCreditRequestSummaryList()
        {
            try
            {
                List<CreditRequestSummary> creditList = db.CreditRequestSummary.ToList();
                int recordCount = creditList.Count;
                return Json(new { Result = "OK", Records = creditList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddCreditRequestSummary(CreditRequestSummary creList)
        {
            try
            {
                creList.CreationDate = DateTime.Now;
                creList.DateLastUpdated = DateTime.Now;
                creList.UpdateUserId = "Ayorinde";

                // var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                //applist.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Credit Request Summary " + creList.Facility, UserId = "Ayorinde" };
                db.Audit.Add(userAudit);
                db.CreditRequestSummary.Add(creList);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = creList });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditCreditRequestSummary(CreditRequestSummary crrList)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                crrList.DateLastUpdated = DateTime.Now;
                crrList.UpdateUserId = "Ayorinde";
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Credit Request Summary " + crrList.Facility, UserId = "Ayorinde" };
                db.Audit.Add(userAudit);
                db.Entry(crrList).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteCreditRequestSummary(int FacilityID)
        {
            try
            {
                System.Threading.Thread.Sleep(50);
                CreditRequestSummary crList = db.CreditRequestSummary.Find(FacilityID);
                db.CreditRequestSummary.Remove(crList);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Credit Request Summary " + crList.Facility, UserId = "Ayorinde" };
                db.Audit.Add(userAudit);
                db.SaveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion


        //
        // GET: /CAMTemplate/

        public ActionResult Index()
        {
            try
            {
                string username = Session["Username"].ToString();
                //Return all CAMs for this User.
                var cams = from c in db.CAM where c.InitiatedBy == username select c;
                return View(cams.ToList());
            }
            catch (Exception res)
            {
                Logging.WriteLog(res.Message + ":" + res.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");
            }
          //  return null;
        }

       


        //Insert comment from user into database and approve
        [HttpPost]
        public JsonResult insertComments(string comment)
        {

            try
            {

                //insert comment into database 
                Comment comme = new Comment();
                comme.CAMId = long.Parse(Session["CamId"].ToString());
                comme.Username = Session["Username"].ToString();
                comme.Designation = Session["maindesignation"].ToString();
                comme.Comment1 = comment;
                comme.CommentDate = DateTime.Now;
                comme.EditDate = DateTime.Now;

                db.Comment.Add(comme);

                //Insert into Audit table
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Comment " + comment, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.SaveChanges();

                return Json(new { Result = "OK", Message = "Comment added successfully!", Feedback = retrieveCommentDetails() });
            }

            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult insertComments1(string comment)
        {

            try
            {

                //insert comment into database 
                Comment comme = new Comment();
                //comme.CAMId = long.Parse(Session["cammiid"].ToString());
                comme.CAMId = long.Parse(Session["CAMIId"].ToString());
                comme.Username = Session["Username"].ToString();
                comme.Designation = Session["maindesignation"].ToString();
                comme.Comment1 = comment;
                comme.CommentDate = DateTime.Now;
                comme.EditDate = DateTime.Now;

                db.Comment.Add(comme);

                //Insert into Audit table
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Comment " + comment, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.SaveChanges();

                return Json(new { Result = "OK", Message = "Comment added successfully!", Feedback = retrieveCommentDetails1() });
            }

            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                
                    foreach (string file in Request.Files)
                    {

                        // HttpPostedFileBase hpf = Request.Files[file];
                        //Save file here
                        foreach (var f in files)
                        {
                            //Ensure the file to be uploaded does not exceed 1MB
                            if (f.ContentLength > 1048576)
                            {
                                ViewBag.MaxFileExceeded = "You are required to upload files with content less than 1MB.";
                                return View();
                            }

                            if (f.ContentLength > 0)
                            {
                                long camiiid = 0;
                                string fileName = Path.GetFileName(f.FileName);
                                string path = Path.Combine(Server.MapPath("~/App_Data_Old/CAMDocuments"), fileName);
                                // string path = Path.Combine("C://CAMdocuments", fileName);
                                //string path = Path.Combine(ConfigurationManager.AppSettings["filepath"], fileName);

                                f.SaveAs(path);

                                //Save in the fileinformation table.

                                camiiid = long.Parse(Session["CAMIId"].ToString());

                                string fillename = fileName;
                                string fillepath = ConfigurationManager.AppSettings["filepath2"] + fillename;

                                //Insert into database
                                fileinformation finformation = new fileinformation();
                                finformation.CAMId = camiiid;
                                finformation.filename = fillename;
                                finformation.filepath = fillepath;

                                db.fileinformation.Add(finformation);
                                db.SaveChanges();


                                //  Session["cammiid"] = null;
                            }
                        }
                        Session["cammiid"] = null;
                        Session["CAMId"] = null;
                        Session["CAMIId"] = null;
                        Session["oldcamiid"] = null;
                        return RedirectToAction("Index", "Applicant");
                    }
                
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");

            }
            return null;
        }

        //
        // GET: /CAMTemplate/Details/5

        public ActionResult Details(long id = 0)
        {
            CAM cam = db.CAM.Find(id);
            if (cam == null)
            {
                return HttpNotFound();
            }
            return View(cam);
        }

        //
        // GET: /CAMTemplate/Create

        public ActionResult Create()
        {
            ViewBag.Comment1 = retrieveComments();

            try
            {
                appid = long.Parse(Session["appiid"].ToString());


                var credittype = from ct in db.ApplicantsBrief where ct.ApplicantID == appid select ct;

                if (credittype != null)
                {
                    foreach (var crtype in credittype)
                    {
                        long ApplicantType = long.Parse(crtype.ApplicantType.ToString());

                        var crtyptext = from ctt in db.ApplicantType where ctt.id == ApplicantType select ctt;

                        if (crtyptext != null)
                        {
                            foreach (var crr in crtyptext)
                            {
                                crrtext = crr.descritpion;
                            }
                        }
                    }
                }

                //Add the credit type to the session
                Session["creditypte"] = crrtext;


                crrtext1 = Session["creditypte"].ToString();

                if (crrtext1 == "Commercial")
                {
                    ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                }

                else if (crrtext1 == "Corporate")
                {
                    ViewBag.NextLevelList = new SelectList(CamHelper.GetNextCorporateUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                }

                //run a query to retrieve information from the ApplicantsBrief table and Credit Request Summary Table and put inside the Viewbag.
                try
                {
                    //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                    var applicantsbrief = from ab in db.ApplicantsBrief
                                          join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                          join ef in db.IndustryType on ab.Industry equals ef.id
                                          where ab.ApplicantType == cd.id & ab.Industry == ef.id
                                          && ab.ApplicantID == appid
                                          select new
                                          {
                                              cd.descritpion,
                                              ab.LegalIdentityName,
                                              ab.YearOfIncorporation,
                                              ab.RCNumber,
                                              ab.BusinessAddress,
                                              ab.NameOfAuditors,
                                              ab.NatureOfBusiness,
                                              ef.industrydescription,
                                              ab.ProductRange,
                                              ab.MajorClients
                                          };


                    List<appbrief> appbrief = new List<appbrief>();
                    foreach (var s in applicantsbrief)
                    {
                        appbrief.Add(new appbrief
                        {
                            apptype = s.descritpion,
                            legalidentityname = s.LegalIdentityName,
                            yearofincorporation = s.YearOfIncorporation,
                            rcnumber = s.RCNumber,
                            businessaddress = s.BusinessAddress,
                            nameofauditors = s.NameOfAuditors,
                            natureofbusiness = s.NatureOfBusiness,
                            industry = s.industrydescription,
                            productrange = s.ProductRange,
                            majorclients = s.MajorClients
                        });
                    }



                    ViewBag.ApplicantsBriefInfo = appbrief;

                    if (Session["oldcamiid"] != null)
                    {
                        cid = long.Parse(Session["oldcamiid"].ToString());
                    }
                    var creditrequest = from cr in db.CreditRequestSummary
                                        join ab in db.FacilityType on cr.Facility equals ab.id
                                        join dd in db.ApplicationType on cr.ApplicationType equals dd.id


                                        where cr.Facility == ab.id
                                        && cr.ApplicantID == appid
                                        && cr.ApplicationType == dd.id
                                        && cr.CAMId == cid

                                        select new
                                        {
                                            cr.CAMId,
                                            ab.description,
                                            cr.FacilityAmount,
                                            dd.applicationdescription,
                                            cr.Purpose,
                                            cr.Tenor,
                                            cr.InterestRate,
                                            cr.ProcessingFee,
                                            cr.ManagementFee,
                                            cr.LCCommission,
                                            cr.OtherCharges,
                                            cr.COTRate,
                                            cr.SourceOfRepayment,
                                            cr.ModeOfRepayment,
                                            cr.Security,
                                            cr.Guarantee
                                        };
                    List<creditrequest> creditrequestt = new List<creditrequest>();
                    foreach (var t in creditrequest)
                    {
                        creditrequestt.Add(new creditrequest
                        {
                            camid = long.Parse(t.CAMId.ToString()),
                            facilitytype = t.description,
                            facilityamount = t.FacilityAmount,
                            applicanttype = t.applicationdescription,
                            purpose = t.Purpose,
                            tenor = t.Tenor,
                            interestrate = t.InterestRate,
                            processingfee = t.ProcessingFee,
                            managementfee = t.ManagementFee,
                            lccommission = t.LCCommission,
                            othercharges = t.OtherCharges,
                            cotrate = t.COTRate,
                            sourceofrepayment = t.SourceOfRepayment,
                            modeofrepayment = t.ModeOfRepayment,
                            security = t.Security,
                            guarantee = t.Guarantee
                        });
                    }

                    ViewBag.CreditRequestInfo = creditrequestt;
                }
                catch (Exception es)
                {
                    Logging.WriteLog(es.Message + ":" + es.StackTrace);
                    //Redirect user to error page.
                    return RedirectToAction("Exception","CAMForm");
                }




                //id = long.Parse(Session["CAMId"].ToString());
                //Session["applicantid"] = id;


                //Get the type of Credit it is

            }
            catch (Exception rrrr)
            {
                Logging.WriteLog(rrrr.Message + ":" + rrrr.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception","CAMForm");
            }

            return View();
        }

        //
        // POST: /CAMTemplate/Create

        //[HttpPost]
        [HttpPost, ValidateInput(false)]
        //  [ValidateAntiForgeryToken]
        public ActionResult Create(CAM cam, string NextLevelSupervisor, string SelectAction, decimal TotalFacilityAmount)
        {
            
            try
            {
                //If a save action was selected
                if (SelectAction == "Save")
                {
                    //Retrieve CAMId
                    cam.ApplicantID = long.Parse(Session["appiid"].ToString());
                    cam.CAMId = long.Parse(Session["CAMIId"].ToString());
                   // cam.CAMId = DateTime.Now.Millisecond; 
                    // cam.ApplicantID = long.Parse(Session["applicantid"].ToString());
                    cam.DateLastUpdated = DateTime.Now;
                    cam.UpdateUserId = Session["Username"].ToString();
                    cam.Status = "Pending";
                    cam.InitiatedBy = Session["Username"].ToString();
                    //cam.CreationDate = DateTime.Now;
                    cam.CreationDate = DateTime.Now.Date;
                    cam.DestinationUserId = Session["Username"].ToString();
                    cam.RegionProcessedFrom = Session["Region"].ToString();
                    cam.ZoneProcessedFrom = Session["ZONE"].ToString();
                    cam.BranchProcessedFrom = Session["BRANCH"].ToString();

                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM saved by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    //Add Action Method
                    // CAM camm = db.CAM.Find(cam.CAMId);

                    ActionAudit ab = new ActionAudit { CAMId = cam.CAMId, Action = "CAM saved by " + Session["Username"].ToString(), FromUserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString(), DateActionPerformed = DateTime.Now };
                    db.ActionAudit.Add(ab);
                }
                else if (SelectAction == "Submit")
                {
                    //Find out what kind of Credit it is

                    cam.ApplicantID = long.Parse(Session["appiid"].ToString());
                    cam.CAMId = long.Parse(Session["CAMIId"].ToString());

                    cam.DateLastUpdated = DateTime.Now;
                    cam.UpdateUserId = Session["Username"].ToString();
                    cam.Status = "Pending";
                    cam.InitiatedBy = Session["Username"].ToString();
                    // cam.CreationDate = DateTime.Now;
                    cam.CreationDate = DateTime.Now.Date;
                    cam.DestinationUserId = NextLevelSupervisor;
                    cam.RegionProcessedFrom = Session["Region"].ToString();
                    cam.ZoneProcessedFrom = Session["ZONE"].ToString();
                    cam.BranchProcessedFrom = Session["BRANCH"].ToString();

                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "New CAM Created by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };

                    //  CAM camm = db.CAM.Find(cam.CAMId);
                    ActionAudit aa = new ActionAudit { CAMId = cam.CAMId, Action = "New CAM Created by " + Session["Username"].ToString() + "and sent ", DestinationUserId = "to " + NextLevelSupervisor, DateActionPerformed = DateTime.Now };
                    db.ActionAudit.Add(aa);
                }

                if (ModelState.IsValid)
                {
                    db.CAM.Add(cam);
                    //Add Action Audit

                    db.SaveChanges();

                    //Retrieve Next Level Supervisor e-mail address
                    //var emails = from ee in db.CAM_User where ee.Username == NextLevelSupervisor select ee;
                    //if (emails != null)
                    //{
                    //    var emailaddress = emails.ToList();
                    //    foreach (var useremail in emailaddress)
                    //    {
                    //        useremailaddress = useremail.EmailAddress;
                    //    }

                    //}
                    ////Send e-mail to user

                    //new CamHelper().alertNextOnQueue(NextLevelSupervisor, useremailaddress, Session["Username"].ToString());


                    //return RedirectToAction("Index");
                    if (SelectAction == "Save")
                    {
                        originatinguser = Session["Username"].ToString();
                        var emails = from ee in db.CAM_User where ee.Username == originatinguser select ee;
                        if (emails != null)
                        {
                            var emailaddress = emails.ToList();
                            foreach (var useremail in emailaddress)
                            {
                                useremailaddress = useremail.EmailAddress;
                            }

                        }
                    }

                    else if (SelectAction == "Submit")
                    {

                        var emails = from ee in db.CAM_User where ee.Username == NextLevelSupervisor select ee;
                        if (emails != null)
                        {
                            var emailaddress = emails.ToList();
                            foreach (var useremail in emailaddress)
                            {
                                useremailaddress = useremail.EmailAddress;
                            }

                        }
                    }
                    //Send e-mail to user
                    if (SelectAction == "Save")
                    {

                        //   new CamHelper().alertNextOnQueue(Session["Username"].ToString(), useremailaddress, Session["Username"].ToString());

                        Session["CAMId"] = null;

                        return RedirectToAction("Index", "Applicant");
                    }
                    else if (SelectAction == "Submit")
                    {
                        new CamHelper().alertNextOnQueue(NextLevelSupervisor, useremailaddress, Session["Username"].ToString());

                        // return RedirectToAction("Index", "Applicant");

                        Session["CAMId"] = null;
                        //Attach Supporting documents
                        return RedirectToAction("Index");



                    }
                    //return RedirectToAction("Index", "Applicant");
                }

                return View(cam);
            }

            catch (Exception ep)
            {
                Logging.WriteLog(ep.Message + ":" + ep.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception","CAMForm");
            }

        }

        public ActionResult Exception()
        {
            return View();
        }

        private List<Comment> retrieveComments()
        {

            int camId = int.Parse(Session["oldcamiid"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;
            return com.ToList();
        }

        private List<Comment> retrieveComments1()
        {
            int camId = int.Parse(Session["CAMIId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;
            return com.ToList();
        }
        private string retrieveCommentDetails()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;

            string tableStructure = "<table class=\"tblboda\"><tr><th>Username</th><th>Designation</th><th>Comment</th><th>Date</th></tr>";
            foreach (var f in com)
            {
                tableStructure = tableStructure + "<tr><td>" + f.Username + "</td>" + "<td>" + f.Designation + "</td>" + "<td>" + f.Comment1 + "</td>" + "<td>" + f.CommentDate + "</td></tr>";
            }
            tableStructure = tableStructure + "</table>";
            return tableStructure;

        }

        private string retrieveCommentDetails1()
        {
            int camId = int.Parse(Session["CAMIId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;

            string tableStructure = "<table class=\"tblboda\"><tr><th>Username</th><th>Designation</th><th>Comment</th><th>Date</th></tr>";
            foreach (var f in com)
            {
                tableStructure = tableStructure + "<tr><td>" + f.Username + "</td>" + "<td>" + f.Designation + "</td>" + "<td>" + f.Comment1 + "</td>" + "<td>" + f.CommentDate + "</td></tr>";
            }
            tableStructure = tableStructure + "</table>";
            return tableStructure;

        }


        //
        // GET: /CAMTemplate/Edit/5

        public ActionResult Edit(long id = 0)
        {
            long cccid = long.Parse(Session["CAMIId"].ToString());
           // Session["oldcamiid"] = id;
            id = long.Parse(Session["oldcamiid"].ToString());

            var totalfacilityamount = db.CreditRequestSummary.Where(s => s.CAMId == cccid).Select(b => b.FacilityAmount).Sum();

            ViewBag.TotalCommitment = totalfacilityamount;

            long newcamid = 0;
            try
            {

                //  ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()),"Username","Username");
                // return View()
                CAM cam = db.CAM.Find(id);

              //  Session["cammid"] = cam.CAMId;
               // Session["appid"] = cam.ApplicantID;

              //  newcamid = cam.CAMId = DateTime.Now.Millisecond;

               // Session["cammiid"] = newcamid;
              //  Session["oldcamid"] = id;

                ViewBag.Comment1 = retrieveComments1();

                if (cam == null)
                {
                    return HttpNotFound();
                }

                appid = long.Parse(Session["appiid"].ToString());

                // appid = long.Parse(Session["appid"].ToString());


                var credittype = from ct in db.ApplicantsBrief where ct.ApplicantID == appid select ct;

                if (credittype != null)
                {
                    foreach (var crtype in credittype)
                    {
                        long ApplicantType = long.Parse(crtype.ApplicantType.ToString());

                        var crtyptext = from ctt in db.ApplicantType where ctt.id == ApplicantType select ctt;

                        if (crtyptext != null)
                        {
                            foreach (var crr in crtyptext)
                            {
                                crrtext = crr.descritpion;
                            }
                        }
                    }
                }

                //Add the credit type to the session
                Session["creditypte"] = crrtext;


                crrtext1 = Session["creditypte"].ToString();

                if (crrtext1 == "Commercial")
                {
                    //  ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                    ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");

                }

                else if (crrtext1 == "Corporate")
                {
                    ViewBag.NextLevelList = new SelectList(CamHelper.GetNextCorporateUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                }




                //run a query to retrieve information from the ApplicantsBrief table and Credit Request Summary Table and put inside the Viewbag.
                try
                {
                    //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                    var applicantsbrief = from ab in db.ApplicantsBrief
                                          join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                          join ef in db.IndustryType on ab.Industry equals ef.id
                                          where ab.ApplicantType == cd.id & ab.Industry == ef.id
                                          && ab.ApplicantID == appid
                                          select new
                                          {
                                              cd.descritpion,
                                              ab.LegalIdentityName,
                                              ab.YearOfIncorporation,
                                              ab.RCNumber,
                                              ab.BusinessAddress,
                                              ab.NameOfAuditors,
                                              ab.NatureOfBusiness,
                                              ef.industrydescription,
                                              ab.ProductRange,
                                              ab.MajorClients
                                          };


                    List<appbrief> appbrief = new List<appbrief>();
                    foreach (var s in applicantsbrief)
                    {
                        appbrief.Add(new appbrief
                        {
                            apptype = s.descritpion,
                            legalidentityname = s.LegalIdentityName,
                            yearofincorporation = s.YearOfIncorporation,
                            rcnumber = s.RCNumber,
                            businessaddress = s.BusinessAddress,
                            nameofauditors = s.NameOfAuditors,
                            natureofbusiness = s.NatureOfBusiness,
                            industry = s.industrydescription,
                            productrange = s.ProductRange,
                            majorclients = s.MajorClients
                        });
                    }


                    ViewBag.ApplicantsBriefInfo = appbrief;
                    if (Session["CAMIId"] != null)
                    {

                        cid = long.Parse(Session["CAMIId"].ToString());
                        
                    }
                    //else if(Session["oldcamid"] !=null)
                    //{
                    //    cid = long.Parse(Session["oldcamid"].ToString());

                    //}
                    


                  //  long ciid = long.Parse(Session["cammiid"].ToString());

                    var creditrequest = from cr in db.CreditRequestSummary
                                        join ab in db.FacilityType on cr.Facility equals ab.id
                                        join dd in db.ApplicationType on cr.ApplicationType equals dd.id


                                        where cr.Facility == ab.id
                                        && cr.ApplicantID == appid
                                        && cr.ApplicationType == dd.id
                                        && cr.CAMId == cid

                                        select new
                                        {
                                            cr.CAMId,
                                            ab.description,
                                            cr.FacilityAmount,
                                            dd.applicationdescription,
                                            cr.Purpose,
                                            cr.Tenor,
                                            cr.InterestRate,
                                            cr.ProcessingFee,
                                            cr.ManagementFee,
                                            cr.LCCommission,
                                            cr.OtherCharges,
                                            cr.COTRate,
                                            cr.SourceOfRepayment,
                                            cr.ModeOfRepayment,
                                            cr.Security,
                                            cr.Guarantee
                                        };
                    List<creditrequest> creditrequestt = new List<creditrequest>();
                    foreach (var t in creditrequest)
                    {
                        creditrequestt.Add(new creditrequest
                        {
                            camid = long.Parse(t.CAMId.ToString()),
                            facilitytype = t.description,
                            facilityamount = t.FacilityAmount,
                            applicanttype = t.applicationdescription,
                            purpose = t.Purpose,
                            tenor = t.Tenor,
                            interestrate = t.InterestRate,
                            processingfee = t.ProcessingFee,
                            managementfee = t.ManagementFee,
                            lccommission = t.LCCommission,
                            othercharges = t.OtherCharges,
                            cotrate = t.COTRate,
                            sourceofrepayment = t.SourceOfRepayment,
                            modeofrepayment = t.ModeOfRepayment,
                            security = t.Security,
                            guarantee = t.Guarantee
                        });
                    }

                    ViewBag.CreditRequestInfo = creditrequestt;
                }
                catch (Exception es)
                {
                    Logging.WriteLog(es.Message + ":" + es.StackTrace);
                    //Redirect user to error page.
                    return RedirectToAction("Exception","CAMForm");
                }

                return View(cam);
            }
            catch (Exception zz)
            {
                Logging.WriteLog(zz.Message + ":" + zz.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception","CAMForm");
            }
        }
        //
        // POST: /CAMTemplate/Edit/5

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CAM cam, string NextLevelSupervisor, string SelectAction, string ApplicantID, decimal TotalFacilityAmount)
        {
            try
            {
                //If a save action was selected
                if (SelectAction == "Save")
                {
                    if (Session["CAMIId"] != null)
                    {
                        cam.CAMId = long.Parse(Session["CAMIId"].ToString());
                    }
                    //else
                    //{
                    //cam.CAMId = long.Parse(Session["cammiid"].ToString());
                    //}
                    cam.ApplicantID = long.Parse(ApplicantID.ToString());
                    cam.DateLastUpdated = DateTime.Now;
                    cam.UpdateUserId = Session["Username"].ToString();
                    cam.Status = "Pending";
                    cam.InitiatedBy = Session["Username"].ToString();
                    cam.CreationDate = DateTime.Now;
                    cam.DestinationUserId = Session["Username"].ToString();
                    cam.RegionProcessedFrom = Session["Region"].ToString();
                    cam.ZoneProcessedFrom = Session["ZONE"].ToString();
                    cam.BranchProcessedFrom = Session["BRANCH"].ToString();

                    // CAM camm = db.CAM.Find(cam.CAMId);

                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM saved by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    db.Audit.Add(userAudit);

                    ActionAudit aa = new ActionAudit { CAMId = cam.CAMId, Action = "CAM edited by " + Session["Username"].ToString(), DateActionPerformed = DateTime.Now, Designation = Session["maindesignation"].ToString() };
                    db.ActionAudit.Add(aa);
                }
                else if (SelectAction == "Submit")
                {
                   // if (Session["cammiid"] !=null)
                   // {
                   //cam.CAMId = long.Parse(Session["cammiid"].ToString());
                   // }
                   // else
                   // {
                         cam.CAMId = long.Parse(Session["CAMIId"].ToString());
                   // }
                  //  cam.CAMId = DateTime.Now.Millisecond; 
                    cam.ApplicantID = long.Parse(ApplicantID.ToString());
                    cam.DateLastUpdated = DateTime.Now;
                    cam.UpdateUserId = Session["Username"].ToString();
                    cam.Status = "Pending";
                    cam.InitiatedBy = Session["Username"].ToString();
                    cam.CreationDate = DateTime.Now;
                    cam.DestinationUserId = NextLevelSupervisor;
                    cam.RegionProcessedFrom = Session["Region"].ToString();
                    cam.ZoneProcessedFrom = Session["ZONE"].ToString();
                    cam.BranchProcessedFrom = Session["BRANCH"].ToString();

                    // CAM camm = db.CAM.Find(cam.CAMId);
                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM submitted by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    db.Audit.Add(userAudit);

                    ActionAudit aa = new ActionAudit { CAMId = cam.CAMId, Action = "New CAM created by " + Session["Username"].ToString() + "and sent ", DestinationUserId = "To " + NextLevelSupervisor, DateActionPerformed = DateTime.Now, Designation = Session["maindesignation"].ToString() };
                    db.ActionAudit.Add(aa);
                }

                if (ModelState.IsValid)
                {

                    //db.Entry(cam).State = EntityState.Modified;

                    //db.SaveChanges();
                    db.CAM.Add(cam);
                    db.SaveChanges();

                    //Retrieve Next Level Supervisor e-mail address
                    if (SelectAction == "Save")
                    {
                        originatinguser = Session["Username"].ToString();
                        var emails = from ee in db.CAM_User where ee.Username == originatinguser select ee;
                        if (emails != null)
                        {
                            var emailaddress = emails.ToList();
                            foreach (var useremail in emailaddress)
                            {
                                useremailaddress = useremail.EmailAddress;
                            }

                        }
                    }

                    else if (SelectAction == "Submit")
                    {

                        var emails = from ee in db.CAM_User where ee.Username == NextLevelSupervisor select ee;
                        if (emails != null)
                        {
                            var emailaddress = emails.ToList();
                            foreach (var useremail in emailaddress)
                            {
                                useremailaddress = useremail.EmailAddress;
                            }

                        }
                    }
                    //Send e-mail to user
                    if (SelectAction == "Save")
                    {

                        //   new CamHelper().alertNextOnQueue(Session["Username"].ToString(), useremailaddress, Session["Username"].ToString());

                        return RedirectToAction("Index", "Applicant");
                    }
                    else if (SelectAction == "Submit")
                    {
                        new CamHelper().alertNextOnQueue(NextLevelSupervisor, useremailaddress, Session["Username"].ToString());

                        // return RedirectToAction("Index", "Applicant");


                        //Attach Files
                        return RedirectToAction("Index");

                    }
                }
                return View(cam);
            }
            catch (Exception t)
            {
                Logging.WriteLog(t.Message + ":" + t.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception","CAMForm");

            }
        }

        //
        // GET: /CAMTemplate/Delete/5

        public ActionResult Delete(long id = 0)
        {
            CAM cam = db.CAM.Find(id);
            if (cam == null)
            {
                return HttpNotFound();
            }
            return View(cam);
        }

        //
        // POST: /CAMTemplate/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CAM cam = db.CAM.Find(id);
            db.CAM.Remove(cam);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}