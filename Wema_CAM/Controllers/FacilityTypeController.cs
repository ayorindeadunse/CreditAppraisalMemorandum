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
    public class FacilityTypeController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();


        #region JSON

        public JsonResult GetFacilityTypeList()
        {
            try
            {
                List<FacilityType> FacilityTypeList = db.FacilityType.ToList();
                int recordCount = FacilityTypeList.Count;
                return Json(new { Result = "OK", Records = FacilityTypeList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddFacilityType(FacilityType FacType)
        {
            try
            {
                //indList.CreationDate = DateTime.Now;
                //indList.DateLastUpdated = DateTime.Now;
                //indList.UpdateUserId = "Ayorinde";

                // var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                //applist.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Industry Type Information " + FacType.description, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.FacilityType.Add(FacType);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = FacType });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditFacilityType(FacilityType FacType)
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
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Industry Type Information " + FacType.description, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.Entry(FacType).State = EntityState.Modified;
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
        public JsonResult DeleteFacilityType(int indID)
        {
            try
            {
                Thread.Sleep(50);
                FacilityType FacList = db.FacilityType.Find(indID);
                db.FacilityType.Remove(FacList);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Industry Type information " + FacList.description, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
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
        // GET: /FacilityType/

        public ActionResult Index()
        {
            return View(db.FacilityType.ToList());
        }

        //
        // GET: /FacilityType/Details/5

        public ActionResult Details(long id = 0)
        {
            FacilityType facilitytype = db.FacilityType.Find(id);
            if (facilitytype == null)
            {
                return HttpNotFound();
            }
            return View(facilitytype);
        }

        //
        // GET: /FacilityType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FacilityType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FacilityType facilitytype)
        {
            if (ModelState.IsValid)
            {
                db.FacilityType.Add(facilitytype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(facilitytype);
        }

        //
        // GET: /FacilityType/Edit/5

        public ActionResult Edit(long id = 0)
        {
            FacilityType facilitytype = db.FacilityType.Find(id);
            if (facilitytype == null)
            {
                return HttpNotFound();
            }
            return View(facilitytype);
        }

        //
        // POST: /FacilityType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FacilityType facilitytype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facilitytype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facilitytype);
        }

        //
        // GET: /FacilityType/Delete/5

        public ActionResult Delete(long id = 0)
        {
            FacilityType facilitytype = db.FacilityType.Find(id);
            if (facilitytype == null)
            {
                return HttpNotFound();
            }
            return View(facilitytype);
        }

        //
        // POST: /FacilityType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FacilityType facilitytype = db.FacilityType.Find(id);
            db.FacilityType.Remove(facilitytype);
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