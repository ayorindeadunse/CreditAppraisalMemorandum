using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;

namespace Wema_CAM.Controllers
{
    public class CreditRequestController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();
        string username = null;
        long CAMId = 0;
        long camid = 0;
        long camid1 = 0;

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

        public JsonResult GetApplicationType()
        {
            try
            {
                var applicanttype = db.ApplicationType.Select(
                    c => new { DisplayText = c.applicationdescription, Value = c.id });
                return Json(new { Result = "OK", Options = applicanttype });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetFacilityType()
        {
            try
            {
                var applicanttype = db.FacilityType.Select(
                    c => new { DisplayText = c.description, Value = c.id });
                return Json(new { Result = "OK", Options = applicanttype });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetCreditRequestSummaryList()
        {
            //List<long> processedcamids = null;
            //List<long> othercamids = null;
           long appid = long.Parse(Session["appid"].ToString());
           long caaaamid = long.Parse(Session["CAMId"].ToString());
            try
            {
               List<CreditRequestSummary> creditList = db.CreditRequestSummary.Where(c => c.CAMId == caaaamid).ToList();
            
                
                int recordCount = creditList.Count;
                if (creditList == null)
                {
                    recordCount = 0;
                }
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
            
               creList.ApplicantID = long.Parse(Session["appid"].ToString());
                creList.CAMId = long.Parse(Session["CAMId"].ToString());
                creList.CreationDate = DateTime.Now.Date;
                creList.DateLastUpdated = DateTime.Now;
                creList.UpdateUserId = Session["Username"].ToString();

                // var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                //applist.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please ensure that the Facility Amount is numeric." });
                }

                else if (creList.FacilityAmount == null)
                {

                    return Json(new { Result = "ERROR", Message = "The Facility Amount field is required." });
                }

                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Credit Request Summary " + creList.ApplicationType, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
               db.Audit.Add(userAudit);
                db.CreditRequestSummary.Add(creList);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);

                //After adding the fresh Credit Request Summary, then refresh the list with the Credit Request Summary of the current CAM.

                
               
                
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
                crrList.ApplicantID = long.Parse(Session["appid"].ToString());
                crrList.CAMId = long.Parse(Session["CAMId"].ToString());
                //region.CreationDate = DateTime.Now;
                crrList.DateLastUpdated = DateTime.Now;
                crrList.UpdateUserId = Session["Username"].ToString();
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                else if (crrList.FacilityAmount == null)
                {

                    return Json(new { Result = "ERROR", Message = "The Facility Amount field is required." });
                }

                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Credit Request Summary " + crrList.Facility, UserId = Session["Username"].ToString()};
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
                Thread.Sleep(50);
                CreditRequestSummary crList = db.CreditRequestSummary.Find(FacilityID);
                db.CreditRequestSummary.Remove(crList);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Credit Request Summary " + crList.Facility, UserId = Session["Username"].ToString() };
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
        // GET: /CreditRequest/

        public ActionResult Index()
        {
            return View(db.CreditRequestSummary.ToList());
        }

        //
        // GET: /CreditRequest/Details/5

        public ActionResult Details(long id = 0)
        {
            CreditRequestSummary creditrequestsummary = db.CreditRequestSummary.Find(id);
            if (creditrequestsummary == null)
            {
                return HttpNotFound();
            }
            return View(creditrequestsummary);
        }

        //
        // GET: /CreditRequest/Create

        public ActionResult Create(long id)
        {
            Session["appid"] = id;
            CAMId = DateTime.Now.Millisecond;
            Session["CAMId"] = CAMId;
            
            return RedirectToAction("index", "CreditRequest");
           // return View();
        }

        //
        // POST: /CreditRequest/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreditRequestSummary creditrequestsummary)
        {
            if (ModelState.IsValid)
            {
                db.CreditRequestSummary.Add(creditrequestsummary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(creditrequestsummary);
        }

        //
        // GET: /CreditRequest/Edit/5

        public ActionResult Edit(long id = 0)
        {
            CreditRequestSummary creditrequestsummary = db.CreditRequestSummary.Find(id);
            if (creditrequestsummary == null)
            {
                return HttpNotFound();
            }
            return View(creditrequestsummary);
        }

        //
        // POST: /CreditRequest/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreditRequestSummary creditrequestsummary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(creditrequestsummary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(creditrequestsummary);
        }

        //
        // GET: /CreditRequest/Delete/5

        public ActionResult Delete(long id = 0)
        {
            CreditRequestSummary creditrequestsummary = db.CreditRequestSummary.Find(id);
            if (creditrequestsummary == null)
            {
                return HttpNotFound();
            }
            return View(creditrequestsummary);
        }

        //
        // POST: /CreditRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CreditRequestSummary creditrequestsummary = db.CreditRequestSummary.Find(id);
            db.CreditRequestSummary.Remove(creditrequestsummary);
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