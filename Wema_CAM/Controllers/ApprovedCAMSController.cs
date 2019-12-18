using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;
using PagedList;

namespace Wema_CAM.Controllers
{
    public class ApprovedCAMSController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        //
        // GET: /ApprovedCAMS/

        public ActionResult Index(int?page)
        {
            try
            {
                string user = Session["Username"].ToString();

                //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                var pendingCAMs = from ca in db.CAM
                                  // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
                                  join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
                                  join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
                                  join ft in db.FacilityType on bc.Facility equals ft.id


                                  //where bc.Facility == ft.id
                                  // && ca.ApplicantID == ba.ApplicantID
                                  //  && ca.ApplicantID == bc.ApplicantID
                                  // && ca.CAMId == ab.CAMId
                                  where ca.CAMId == bc.CAMId
                                    && ca.InitiatedBy == user
                                    && ca.Status == "Approved"
                                    



                                  //   && ca.DestinationUserId != ""

                                  select new
                                  {
                                      ca.CAMId,
                                      ca.InitiatedBy,
                                      ca.DestinationUserId,
                                      ba.LegalIdentityName,
                                      ft.description,
                                      ca.TotalFacilityAmount,
                                      ca.CreationDate,
                                      ca.DateLastUpdated,
                                      ca.Status
                                  };
                // var distinctcams =  pendingCAMs.Distinct();
                var distinctcams1 = pendingCAMs.OrderByDescending(s => s.CreationDate);

                List<PendingCamList> pendingcams1 = new List<PendingCamList>();
                foreach (var g in distinctcams1)
                {
                    pendingcams1.Add(new PendingCamList
                    {
                        camid = g.CAMId,
                        initiatedby = g.InitiatedBy,
                        destinationuser = g.DestinationUserId,
                        customername = g.LegalIdentityName,
                        facilitytype = g.description,
                        facilityamount = g.TotalFacilityAmount.ToString(),
                        dateinitiated = DateTime.Parse(g.CreationDate.ToString()),
                        datelastupdated = DateTime.Parse(g.DateLastUpdated.ToString()),
                        status = g.Status

                    });

                }


                //int pageSize = 10;

                //int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


                ////    ViewBag.pendingcams1.ToPagedList(pageNumber, pageSize);

              

                //return View(pendingcams1.ToPagedList(pageNumber, pageSize));
                return View(pendingcams1);

            }
            catch (Exception ss)
            {
                Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }
           // return View(db.CAM.ToList());
           // return View(Wema_CAM.PendingCamList);
           // return PartialView("~/Views/Shared/_ApprovedCAMs.cshtml");
          //  return null;
            return null;
        }

        public ActionResult Index(int? page, string customersearch, string CAMID)
        {
            if (CAMID != "")
            {

                long CAMMID = long.Parse(CAMID);


                try
                {
                    string user = Session["Username"].ToString();

                    //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                    var pendingCAMs = from ca in db.CAM
                                      // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
                                      join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
                                      join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
                                      join ft in db.FacilityType on bc.Facility equals ft.id


                                      //where bc.Facility == ft.id
                                      // && ca.ApplicantID == ba.ApplicantID
                                      //  && ca.ApplicantID == bc.ApplicantID
                                      // && ca.CAMId == ab.CAMId
                                      where ca.CAMId == bc.CAMId
                                        && ca.InitiatedBy == user
                                        && ca.Status == "Approved"
                                        && ca.CAMId == CAMMID



                                      //   && ca.DestinationUserId != ""

                                      select new
                                      {
                                          ca.CAMId,
                                          ca.InitiatedBy,
                                          ca.DestinationUserId,
                                          ba.LegalIdentityName,
                                          ft.description,
                                          ca.TotalFacilityAmount,
                                          ca.CreationDate,
                                          ca.DateLastUpdated,
                                          ca.Status
                                      };
                    // var distinctcams =  pendingCAMs.Distinct();
                    var distinctcams1 = pendingCAMs.OrderByDescending(s => s.CreationDate);

                    List<PendingCamList> pendingcams1 = new List<PendingCamList>();
                    foreach (var g in distinctcams1)
                    {
                        pendingcams1.Add(new PendingCamList
                        {
                            camid = g.CAMId,
                            initiatedby = g.InitiatedBy,
                            destinationuser = g.DestinationUserId,
                            customername = g.LegalIdentityName,
                            facilitytype = g.description,
                            facilityamount = g.TotalFacilityAmount.ToString(),
                            dateinitiated = DateTime.Parse(g.CreationDate.ToString()),
                            datelastupdated = DateTime.Parse(g.DateLastUpdated.ToString()),
                            status = g.Status

                        });

                    }


                //    int pageSize = 10;

                //    int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


                ////    ViewBag.pendingcams1.ToPagedList(pageNumber, pageSize);

           //   ViewBag.CAM = pendingcams1;
                //    return View(pendingcams1.ToPagedList(pageNumber, pageSize));
                    return View(pendingcams1);

                }
                catch (Exception ss)
                {
                    Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                    //Redirect user to error page.
                    RedirectToAction("Exception", "CAMForm");
                }
            }
            else
            {
                try
                {
                    string user = Session["Username"].ToString();

                    //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                    var pendingCAMs = from ca in db.CAM
                                      // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
                                      join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
                                      join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
                                      join ft in db.FacilityType on bc.Facility equals ft.id


                                      //where bc.Facility == ft.id
                                      // && ca.ApplicantID == ba.ApplicantID
                                      //  && ca.ApplicantID == bc.ApplicantID
                                      // && ca.CAMId == ab.CAMId
                                      where ca.CAMId == bc.CAMId
                                        && ca.InitiatedBy == user
                                        && ca.Status == "Approved"
                                        && ba.LegalIdentityName.Contains(customersearch)



                                      //   && ca.DestinationUserId != ""

                                      select new
                                      {
                                          ca.CAMId,
                                          ca.InitiatedBy,
                                          ca.DestinationUserId,
                                          ba.LegalIdentityName,
                                          ft.description,
                                          ca.TotalFacilityAmount,
                                          ca.CreationDate,
                                          ca.DateLastUpdated,
                                          ca.Status
                                      };
                    // var distinctcams =  pendingCAMs.Distinct();
                    var distinctcams1 = pendingCAMs.OrderByDescending(s => s.CreationDate);

                    List<PendingCamList> pendingcams1 = new List<PendingCamList>();
                    foreach (var g in distinctcams1)
                    {
                        pendingcams1.Add(new PendingCamList
                        {
                            camid = g.CAMId,
                            initiatedby = g.InitiatedBy,
                            destinationuser = g.DestinationUserId,
                            customername = g.LegalIdentityName,
                            facilitytype = g.description,
                            facilityamount = g.TotalFacilityAmount.ToString(),
                            dateinitiated = DateTime.Parse(g.CreationDate.ToString()),
                            datelastupdated = DateTime.Parse(g.DateLastUpdated.ToString()),
                            status = g.Status

                        });

                    }


                 //   int pageSize = 10;

                 //   int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


                 ////   ViewBag.CAM = pendingcams1.ToPagedList(pageNumber, pageSize);

                 //  // ViewBag.CAM = pendingcams1;
                 //   return View(pendingcams1.ToPagedList(pageNumber,pageSize));

                    return View(pendingcams1);

                }
                catch (Exception ss)
                {
                    Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                    //Redirect user to error page.
                    RedirectToAction("Exception", "CAMForm");
                }
            }

            return null;

        }

        //
        // GET: /ApprovedCAMS/Details/5

        public ActionResult Details(long id = 0)
        {
            CAM cam = db.CAM.Find(id);
            if (cam == null)
            {
                return HttpNotFound();
            }
            return View(cam);
        }

        //
        // GET: /ApprovedCAMS/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ApprovedCAMS/Create

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
        // GET: /ApprovedCAMS/Edit/5

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
        // POST: /ApprovedCAMS/Edit/5

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
        // GET: /ApprovedCAMS/Delete/5

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
        // POST: /ApprovedCAMS/Delete/5

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