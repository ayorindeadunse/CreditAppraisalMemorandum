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
    public class ZoneController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        #region JSON

        public JsonResult GetRegionList()
        {
            try
            {
                var regions = db.Region.Select(
                    c => new { DisplayText = c.RegionName, Value = c.RegionId });
                return Json(new { Result = "OK", Options = regions });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetZoneList()
        {
            try
            {
                List<Zone> ZoneList = db.Zone.ToList();
                int recordCount = ZoneList.Count;
                return Json(new { Result = "OK", Records = ZoneList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            } 
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddZone(Zone zone)
        {
            try
            {
                zone.CreationDate = DateTime.Now;
                zone.DateLastUpdated = DateTime.Now;
                zone.UpdateUserId = Session["Username"].ToString();
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Zone " + zone.ZoneName, UserId = Session["Username"].ToString(), Designation=Session["maindesignation"].ToString()};
                db.Audit.Add(userAudit);
                db.Zone.Add(zone);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = zone });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditZone(Zone zone)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                zone.DateLastUpdated = DateTime.Now;
                zone.UpdateUserId = Session["Username"].ToString();
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Zone " + zone.ZoneName, UserId = Session["Username"].ToString(), Designation=Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.Entry(zone).State = EntityState.Modified;
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
        public JsonResult DeleteZone(int ZoneId)
        {
            try
            {
                Thread.Sleep(50);
                Zone zone = db.Zone.Find(ZoneId);
                db.Zone.Remove(zone);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Zone " + zone.ZoneName, UserId = Session["Username"].ToString(),Designation=Session["maindesignation"].ToString() };
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
        // GET: /Zone/

        public ActionResult Index()
        {
            //var zone = db.Zone.Include(z => z.Region);
            //return View(zone.ToList());
            return View();
        }

        //
        // GET: /Zone/Details/5

        public ActionResult Details(int id = 0)
        {
            Zone zone = db.Zone.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            return View(zone);
        }

        //
        // GET: /Zone/Create

        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Region, "RegionId", "RegionName");
            return View();
        }

        //
        // POST: /Zone/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Zone zone)
        {
            if (ModelState.IsValid)
            {
                db.Zone.Add(zone);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.RegionId = new SelectList(db.Region, "RegionId", "RegionName", zone.RegionId);
            return View(zone);
        }

        //
        // GET: /Zone/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Zone zone = db.Zone.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            //ViewBag.RegionId = new SelectList(db.Region, "RegionId", "RegionName", zone.RegionId);
            return View(zone);
        }

        //
        // POST: /Zone/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Zone zone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.RegionId = new SelectList(db.Region, "RegionId", "RegionName", zone.RegionId);
            return View(zone);
        }

        //
        // GET: /Zone/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Zone zone = db.Zone.Find(id);
            if (zone == null)
            {
                return HttpNotFound();
            }
            return View(zone);
        }

        //
        // POST: /Zone/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Zone zone = db.Zone.Find(id);
            db.Zone.Remove(zone);
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