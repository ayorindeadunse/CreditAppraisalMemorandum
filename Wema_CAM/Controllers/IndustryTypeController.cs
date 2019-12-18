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
    public class IndustryTypeController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        #region JSON

        public JsonResult GetIndustryTypeList()
        {
            try
            {
                List<IndustryType> IndustryTypeList = db.IndustryType.ToList();
                int recordCount = IndustryTypeList.Count;
                return Json(new { Result = "OK", Records = IndustryTypeList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddIndustryType(IndustryType indList)
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
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Industry Type Information " + indList.industrydescription, UserId = Session["Username"].ToString() ,Designation=Session["maindesignation"].ToString() };
                db.Audit.Add(userAudit);
                db.IndustryType.Add(indList);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = indList });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditIndustryType(IndustryType indList)
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
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Industry Type Information " + indList.industrydescription, UserId = Session["Username"].ToString() ,Designation=Session["maindesignation"].ToString()};
               db.Audit.Add(userAudit);
                db.Entry(indList).State = EntityState.Modified;
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
        public JsonResult DeleteIndustryType(int indID)
        {
            try
            {
                Thread.Sleep(50);
                IndustryType indlist = db.IndustryType.Find(indID);
                db.IndustryType.Remove(indlist);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Industry Type information " + indlist.industrydescription, UserId = Session["Username"].ToString() ,Designation=Session["maindesignation"].ToString()};
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
        // GET: /IndustryType/

        public ActionResult Index()
        {
            return View(db.IndustryType.ToList());
        }

        //
        // GET: /IndustryType/Details/5

        public ActionResult Details(long id = 0)
        {
            IndustryType industrytype = db.IndustryType.Find(id);
            if (industrytype == null)
            {
                return HttpNotFound();
            }
            return View(industrytype);
        }

        //
        // GET: /IndustryType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /IndustryType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IndustryType industrytype)
        {
            if (ModelState.IsValid)
            {
                db.IndustryType.Add(industrytype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(industrytype);
        }

        //
        // GET: /IndustryType/Edit/5

        public ActionResult Edit(long id = 0)
        {
            IndustryType industrytype = db.IndustryType.Find(id);
            if (industrytype == null)
            {
                return HttpNotFound();
            }
            return View(industrytype);
        }

        //
        // POST: /IndustryType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IndustryType industrytype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(industrytype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(industrytype);
        }

        //
        // GET: /IndustryType/Delete/5

        public ActionResult Delete(long id = 0)
        {
            IndustryType industrytype = db.IndustryType.Find(id);
            if (industrytype == null)
            {
                return HttpNotFound();
            }
            return View(industrytype);
        }

        //
        // POST: /IndustryType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            IndustryType industrytype = db.IndustryType.Find(id);
            db.IndustryType.Remove(industrytype);
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