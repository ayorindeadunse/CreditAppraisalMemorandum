using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;
//using MvcRazorToPdf;


namespace Wema_CAM.Controllers
{
    public class CAMDetailsController : Controller
    {
        long appid = 0;
        long cid = 0;
        string mainbranch = null;
        private Wema_CAMEntities db = new Wema_CAMEntities();

        //
        // GET: /CAMDetails/

        public ActionResult Index()
        {
            return View(db.CAM.ToList());
        }

        //[HttpPost]
        public ActionResult Download(int id = 0)
        {
            try
            {

                var uploads = (from u in db.fileinformation
                               where u.fileid == id
                               select u.filepath).FirstOrDefault();


                if (uploads != null)
                {
                    // string folder = Path.GetFullPath(uploads);
                    string file = Server.MapPath(@"~/App_Data_Old\") + uploads.ToString().Replace("/", @"\");
                    // string fileextension = file.Split(new char[] { '.' })[2];
                    int len = file.Length;

                    string last = file.Substring(file.Length - 1, 1);

                    if (last == "x")
                    {
                        string fileextension = file.Substring(len - 4);

                        Logging.WriteLog("File Extension: " + fileextension);

                        if (fileextension.Equals("pdf"))
                        {
                            return File(file, "application/pdf");
                        }
                        else if (fileextension.Equals("xls") || fileextension.Equals("xslx"))
                        {
                            return File(file, "application/x-msexcel");
                        }
                        else if (fileextension.Equals("doc") || fileextension.Equals("docx"))
                        {
                            return File(file, "application/msword");
                        }

                        else
                        {
                            ViewBag.FileNotSupported = "The selected file cannot be viewed.";

                            //  return View("Details");
                        }
                    }
                    else
                    {
                        string fileextension = file.Substring(len - 3);
                        Logging.WriteLog("File Extension: " + fileextension);

                        if (fileextension.Equals("pdf"))
                        {
                            return File(file, "application/pdf");
                        }
                        else if (fileextension.Equals("xls") || fileextension.Equals("xslx"))
                        {
                            return File(file, "application/x-msexcel");
                        }
                        else if (fileextension.Equals("doc") || fileextension.Equals("docx"))
                        {
                            return File(file, "application/msword");
                        }

                        else
                        {
                            ViewBag.FileNotSupported = "The selected file cannot be viewed.";

                            //  return View("Details");
                        }
                    }
                }
            }
            catch (Exception ext)
            {
                Logging.WriteLog(ext.Message + ":" + ext.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");
            }
            return null;
        }


        [HttpPost]
        public ActionResult Export(long id = 0)
        {
            return null;
        }
        //
        // GET: /CAMDetails/Details/5

        public ActionResult Details(long id = 0)
        {
         
            CAM cam = db.CAM.Find(id);
            Session["CamId"] = id;
            ViewBag.Comment = retrieveComments();
            ViewBag.Comment1 = retrieveAdditionalComments();

            ViewBag.SupportingDocuments = retrieveSupportingDocuments();

            //Select originating branch

            var origbranch = from b in db.CAM where b.CAMId == id select b;
            if (origbranch != null)
            {
                foreach (var bb in origbranch)
                {
                    mainbranch = bb.BranchProcessedFrom;
                }
            }

            ViewBag.MainBranch = mainbranch;

            if (cam == null)
            {
                return HttpNotFound();
            }

            appid = long.Parse(cam.ApplicantID.ToString());

            //run a query to retrieve information from the ApplicantsBrief table and Credit Request Summary Table and put inside the Viewbag.
            try
            {
                //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                var applicantsbrief = from ab in db.ApplicantsBrief
                                      join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                      join ef in db.IndustryType on ab.Industry equals ef.id
                                      where ab.ApplicantType == cd.id & ab.Industry == ef.id
                                      & ab.ApplicantID == appid
                                    
                                      select new  { cd.descritpion,ab.LegalIdentityName,
                                          ab.YearOfIncorporation,ab.RCNumber,ab.BusinessAddress,
                                      ab.NameOfAuditors, ab.NatureOfBusiness, ef.industrydescription, ab.ProductRange,
                                      ab.MajorClients};

               
                List<appbrief> appbrief = new  List<appbrief>();
                foreach (var s in applicantsbrief)
                {
                    appbrief.Add(new appbrief { apptype=s.descritpion,legalidentityname = s.LegalIdentityName,
                    yearofincorporation = s.YearOfIncorporation, rcnumber = s.RCNumber, businessaddress = s.BusinessAddress,
                    nameofauditors = s.NameOfAuditors, natureofbusiness = s.NatureOfBusiness,industry = s.industrydescription, productrange = s.ProductRange,
                    majorclients = s.MajorClients});
                }


                ViewBag.ApplicantsBriefInfo = appbrief;

                if(Session["CamId"] !=null)
                {
                      cid = long.Parse(Session["CamId"].ToString());
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
                        facilityamount=decimal.Parse(t.FacilityAmount.ToString()),
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
                return RedirectToAction("Exception", "CAMForm");
            }
            return View(cam);
        }

        [HttpPost]
        public ActionResult DetailsPDF(string pdfcontents)
        {
            return null;
            
        }

  

        [HttpGet]
        public ActionResult DetailsPDF(long id = 0)
        {

            CAM cam = db.CAM.Find(id);
            Session["CamId"] = id;
            ViewBag.Comment = retrieveComments();
            ViewBag.Comment1 = retrieveAdditionalComments();

            ViewBag.SupportingDocuments = retrieveSupportingDocuments();

            //Select originating branch

            var origbranch = from b in db.CAM where b.CAMId == id select b;
            if (origbranch != null)
            {
                foreach (var bb in origbranch)
                {
                    mainbranch = bb.BranchProcessedFrom;
                }
            }

            ViewBag.MainBranch = mainbranch;

            if (cam == null)
            {
                return HttpNotFound();
            }

            appid = long.Parse(cam.ApplicantID.ToString());

            //run a query to retrieve information from the ApplicantsBrief table and Credit Request Summary Table and put inside the Viewbag.
            try
            {
                //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                var applicantsbrief = from ab in db.ApplicantsBrief
                                      join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                      join ef in db.IndustryType on ab.Industry equals ef.id
                                      where ab.ApplicantType == cd.id & ab.Industry == ef.id
                                      & ab.ApplicantID == appid

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

                if (Session["CamId"] != null)
                {
                    cid = long.Parse(Session["CamId"].ToString());
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
                        facilityamount = decimal.Parse(t.FacilityAmount.ToString()),
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
                return RedirectToAction("Exception", "CAMForm");
            }
            return View(cam);
        }
        private List<Comment> retrieveComments()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;
            return com.ToList();
        }

        //private List<Comment> retrieveAdditionalComments()
        //{
        //    int camId = int.Parse(Session["CamId"].ToString());
        //    var com = from c in db.AdditionalComment where c.CAMId == camId select c;
        //    return com.ToList();
        //}
        //Retrieve additional comments

        private List<AdditionalComment> retrieveAdditionalComments()
        {

            int camId = int.Parse(Session["CamId"].ToString());
            var com = from c in db.AdditionalComment where c.CAMId == camId select c;
            return com.ToList();
        }

        //Retrieve supporting documents
        private List<fileinformation> retrieveSupportingDocuments()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var sd = from s in db.fileinformation where s.CAMId == camId select s;
            return sd.ToList();
        }
        //
        // GET: /CAMDetails/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CAMDetails/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CAM cam)
        {
            if (ModelState.IsValid)
            {
                db.CAM.Add(cam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cam);
        }

        //
        // GET: /CAMDetails/Edit/5

        public ActionResult Edit(long id = 0)
        {
            CAM cam = db.CAM.Find(id);
            if (cam == null)
            {
                return HttpNotFound();
            }
            return View(cam);
        }

        //
        // POST: /CAMDetails/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CAM cam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cam);
        }

        //
        // GET: /CAMDetails/Delete/5

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
        // POST: /CAMDetails/Delete/5

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