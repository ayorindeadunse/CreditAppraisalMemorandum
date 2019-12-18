using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;

namespace Wema_CAM.Controllers
{
    public class sampleJTEController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        #region JSON

        public JsonResult GetsampleJTEList()
        {
            try
            {
                List<sampleJTE> GetsampleJTEList = db.sampleJTE.ToList();
                int recordCount = GetsampleJTEList.Count;
                return Json(new { Result = "OK", Records = GetsampleJTEList, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddsampleJTE(sampleJTE samplejte, string t)
        {
            try
            {
                samplejte.entry = t;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                //Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Zone " + zone.ZoneName, UserId = "Ayorinde" };
                //db.Audit.Add(userAudit);
                db.sampleJTE.Add(samplejte);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = samplejte });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditsampleJTE(sampleJTE samplejte, string t)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                //zone.DateLastUpdated = DateTime.Now;
                //zone.UpdateUserId = "Ayorinde";
                samplejte.entry = t;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                //Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Zone " + zone.ZoneName, UserId = "Ayorinde" };
                //db.Audit.Add(userAudit);
                db.Entry(samplejte).State = EntityState.Modified;
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
        public JsonResult DeletesampleJTE(int id)
        {
            try
            {
                Thread.Sleep(50);
                sampleJTE samplejte = db.sampleJTE.Find(id);
                db.sampleJTE.Remove(samplejte);
                //Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Zone " + zone.ZoneName, UserId = "Ayorinde" };
                //db.Audit.Add(userAudit);
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
        // GET: /sampleJTE/

        public ActionResult Index()
        {
            return View(db.sampleJTE.ToList());
        }

        //
        // GET: /sampleJTE/Details/5

        public ActionResult Details(int id = 0)
        {
            sampleJTE samplejte = db.sampleJTE.Find(id);
            if (samplejte == null)
            {
                return HttpNotFound();
            }
            return View(samplejte);
        }

        //
        // GET: /sampleJTE/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /sampleJTE/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(sampleJTE samplejte)
        {
            if (ModelState.IsValid)
            {
                db.sampleJTE.Add(samplejte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(samplejte);
        }

        //
        // GET: /sampleJTE/Edit/5

        public ActionResult Edit(int id = 0)
        {
            sampleJTE samplejte = db.sampleJTE.Find(id);
            if (samplejte == null)
            {
                return HttpNotFound();
            }
            return View(samplejte);
        }

        //
        // POST: /sampleJTE/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(sampleJTE samplejte)
        {
            if (ModelState.IsValid)
            {
                db.Entry(samplejte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(samplejte);
        }

        //
        // GET: /sampleJTE/Delete/5

        public ActionResult Delete(int id = 0)
        {
            sampleJTE samplejte = db.sampleJTE.Find(id);
            if (samplejte == null)
            {
                return HttpNotFound();
            }
            return View(samplejte);
        }

        //
        // POST: /sampleJTE/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sampleJTE samplejte = db.sampleJTE.Find(id);
            db.sampleJTE.Remove(samplejte);
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