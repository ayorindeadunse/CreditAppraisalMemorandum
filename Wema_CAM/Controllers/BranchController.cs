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
    public class BranchController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        #region JSON

        public JsonResult GetZoneList()
        {
            try
            {
                var zones = db.Zone.Select(
                    c => new { DisplayText = c.ZoneName, Value = c.ZoneId });
                return Json(new { Result = "OK", Options = zones });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public JsonResult GetBranchList()
        {
            try
            {
                List<Branch> BranchList = db.Branch.ToList();
                int recordCount = BranchList.Count;
                return Json(new { Result = "OK", Records = BranchList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddBranch(Branch branch)
        {
            try
            {
                branch.CreationDate = DateTime.Now;
                branch.DateLastUpdated = DateTime.Now;
               
                branch.UpdateUserId = Session["Username"].ToString();

                var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                branch.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Branch " + branch.BranchName, UserId = Session["Username"].ToString(), Designation=Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.Branch.Add(branch);
                db.SaveChanges();
               
                return Json(new { Result = "OK", Record = branch });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditBranch(Branch branch)
        {
            try
            {
              branch.CreationDate = DateTime.Now;
                branch.DateLastUpdated = DateTime.Now;
                branch.UpdateUserId = Session["Username"].ToString();

                var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                branch.RegionId = region.RegionId;

                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Branch " + branch.BranchName, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.Entry(branch).State = EntityState.Modified;
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
        public JsonResult DeleteBranch(string BranchId)
        {
            try
            {
                Thread.Sleep(50);
                Branch branch = db.Branch.Find(BranchId);
                db.Branch.Remove(branch);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Branch " + branch.BranchName, UserId = Session["Username"].ToString(),Designation=Session["maindesignation"].ToString() };
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
        // GET: /Branch/

        public ActionResult Index()
        {
            return View(db.Branch.ToList());
        }

        //
        // GET: /Branch/Details/5

        public ActionResult Details(string id = null)
        {
            Branch branch = db.Branch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // GET: /Branch/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Branch/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branch.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(branch);
        }

        //
        // GET: /Branch/Edit/5

        public ActionResult Edit(string id = null)
        {
            Branch branch = db.Branch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // POST: /Branch/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(branch);
        }

        //
        // GET: /Branch/Delete/5

        public ActionResult Delete(string id = null)
        {
            Branch branch = db.Branch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        //
        // POST: /Branch/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Branch branch = db.Branch.Find(id);
            db.Branch.Remove(branch);
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