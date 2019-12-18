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
    public class ApplicantTypeController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();


        #region JSON

        public JsonResult GetApplicantTypeList()
        {
            try
            {
                List<ApplicantType> ApplicantTypeList = db.ApplicantType.ToList();
                int recordCount = ApplicantTypeList.Count;
                return Json(new { Result = "OK", Records = ApplicantTypeList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddApplicantType(ApplicantType applist2)
        {
            try
            {
                //applist.CreationDate = DateTime.Now;
                //applist.DateLastUpdated = DateTime.Now;
                //applist.UpdateUserId = "Ayorinde";

                // var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                //applist.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Applicant Information " + applist2.descritpion, UserId = Session["Username"].ToString(),Designation=Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.ApplicantType.Add(applist2);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = applist2 });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditApplicantType(ApplicantType applist)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                //applist.DateLastUpdated = DateTime.Now;
                //applist.UpdateUserId = "Ayorinde";
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Applicant Information " + applist.descritpion, UserId = Session["Username"].ToString(),Designation=Session["maindesignation"].ToString()};
                db.Audit.Add(userAudit);
                db.Entry(applist).State = EntityState.Modified;
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
        public JsonResult DeleteApplicantType(int ApplicantID)
        {
            try
            {
                Thread.Sleep(50);
                ApplicantType applist = db.ApplicantType.Find(ApplicantID);
                db.ApplicantType.Remove(applist);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Branch " + applist.descritpion, UserId = Session["Username"].ToString(),Designation=Session["maindesignation"].ToString() };
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
        // GET: /ApplicantType/

        public ActionResult Index()
        {
            return View(db.ApplicantType.ToList());
        }

        //
        // GET: /ApplicantType/Details/5

        public ActionResult Details(long id = 0)
        {
            ApplicantType applicanttype = db.ApplicantType.Find(id);
            if (applicanttype == null)
            {
                return HttpNotFound();
            }
            return View(applicanttype);
        }

        //
        // GET: /ApplicantType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ApplicantType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplicantType applicanttype)
        {
            if (ModelState.IsValid)
            {
                db.ApplicantType.Add(applicanttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicanttype);
        }

        //
        // GET: /ApplicantType/Edit/5

        public ActionResult Edit(long id = 0)
        {
            ApplicantType applicanttype = db.ApplicantType.Find(id);
            if (applicanttype == null)
            {
                return HttpNotFound();
            }
            return View(applicanttype);
        }

        //
        // POST: /ApplicantType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicantType applicanttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicanttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicanttype);
        }

        //
        // GET: /ApplicantType/Delete/5

        public ActionResult Delete(long id = 0)
        {
            ApplicantType applicanttype = db.ApplicantType.Find(id);
            if (applicanttype == null)
            {
                return HttpNotFound();
            }
            return View(applicanttype);
        }

        //
        // POST: /ApplicantType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ApplicantType applicanttype = db.ApplicantType.Find(id);
            db.ApplicantType.Remove(applicanttype);
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