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
    public class RegionController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();


        #region JSON 

        public JsonResult GetRegionList()
        {
            try
            {
                List<Region> regionList = db.Region.ToList();
                int recordCount = regionList.Count;
                return Json(new { Result = "OK", Records = regionList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            } 
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddRegion(Region region)
        {
            try
            {
                region.CreationDate = DateTime.Now;
                region.DateLastUpdated = DateTime.Now;
                region.UpdateUserId = Session["Username"].ToString();
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added region " + region .RegionName, UserId=Session["Username"].ToString(), Designation=Session["maindesignation"].ToString()};
                db.Audit.Add(userAudit);
                db.Region.Add(region);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = region });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult EditRegion(Region region)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                region.DateLastUpdated = DateTime.Now;
                region.UpdateUserId = Session["Username"].ToString();
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated region " + region.RegionName, UserId = Session["Username"].ToString() ,Designation=Session["maindesignation"].ToString()};
                db.Audit.Add(userAudit);
                db.Entry(region).State = EntityState.Modified;
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
        public JsonResult DeleteRegion(int RegionId)
        {
            try
            {
                Thread.Sleep(50);
                Region region = db.Region.Find(RegionId);
                db.Region.Remove(region);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted region " + region.RegionName, UserId = Session["Username"].ToString(), Designation=Session["maindesignation"].ToString() };
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
        // GET: /Region/

        public ActionResult Index()
        {
            return View(db.Region.ToList());
        }

        //
        // GET: /Region/Details/5

        public ActionResult Details(int id = 0)
        {
            Region region = db.Region.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        //
        // GET: /Region/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Region/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Region.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(region);
        }

        //
        // GET: /Region/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Region region = db.Region.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        //
        // POST: /Region/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(region);
        }

        //
        // GET: /Region/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Region region = db.Region.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        //
        // POST: /Region/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Region region = db.Region.Find(id);
            db.Region.Remove(region);
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