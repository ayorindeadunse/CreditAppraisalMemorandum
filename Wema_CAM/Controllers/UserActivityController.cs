using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;

namespace Wema_CAM.Controllers
{
    public class UserActivityController : Controller
    {
       

        private Wema_CAMEntities db = new Wema_CAMEntities();

        
        //
        // GET: /UserActivity/

        public ActionResult Index()
        {
          //  return View(db.Audit.ToList());
            return View();
        }

        //
        // GET: /UserActivity/Details/5

        public ActionResult Details(long id = 0)
        {
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        //
        // GET: /UserActivity/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UserActivity/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Audit audit)
        {
            if (ModelState.IsValid)
            {
                db.Audit.Add(audit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(audit);
        }

        //
        // GET: /UserActivity/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        //
        // POST: /UserActivity/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Audit audit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(audit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(audit);
        }

        //
        // GET: /UserActivity/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Audit audit = db.Audit.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit);
        }

        //
        // POST: /UserActivity/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Audit audit = db.Audit.Find(id);
            db.Audit.Remove(audit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult SearchUserActivity(string userr, string startdate, string enddate)
        {
            if (userr != "" && startdate != "" && enddate != "")
            {
                return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnUserActivityResults(userr,startdate,enddate) });
            }
            return null;
        }

        private string returnUserActivityResults(string userrr, string starrtdate, string ennddate)
        {
            try
            {
                DateTime starttdate = DateTime.Parse(starrtdate.ToString());
                DateTime endddate = DateTime.Parse(ennddate.ToString());

                var useractivity = from ua in db.Audit where ua.UserId == userrr && ua.ActivityDate >= starttdate && ua.ActivityDate <= endddate select ua;


                if (useractivity != null)
                {
                    string tableStructure = "<table class=\"tblboda1\"><tr><th>Description</th><th>User ID</th><th>Designation</th><th>Activity Date</th>";
                    foreach (var uua in useractivity)
                    {

                        tableStructure = tableStructure + "<tr><td>" + uua.Description + "</td>" + "<td>" + uua.UserId + "</td>" + "<td>" + uua.Designation + "</td>" + "<td>" + uua.ActivityDate + "</td>" + "</tr>";
                    }


                    tableStructure = tableStructure + "</table>";


                    return tableStructure;
                }
              
            }
            catch (Exception ttt)
            {
                Logging.WriteLog(ttt.Message + ":" + ttt.StackTrace);

            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}