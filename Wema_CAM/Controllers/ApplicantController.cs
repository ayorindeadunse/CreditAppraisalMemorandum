using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;
using PagedList;

namespace Wema_CAM.Controllers
{
    public class ApplicantController : Controller
    {
        private Wema_CAMEntities db = new Wema_CAMEntities();

        List<PendingCamList> pendingcams2=null;

       // List<DateTime> createdTime = null;
       // List<decimal> camtimeelapsed = null;
       
        
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

        public JsonResult GetIndustry()
        {
            try
            {
                var industrytype = db.IndustryType.Select(
                    c => new { DisplayText = c.industrydescription, Value = c.id });
                return Json(new { Result = "OK", Options = industrytype });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //Retrieve all the CAMS done by a specific user for display in the Applicant Brief (Existing CAMs) Tab
        private List<CAM> retrieveCAM()
        {
            try
            {
                // int camId = int.Parse(Session["CamId"].ToString());
                string user = Session["Username"].ToString();
                var cams = from c in db.CAM where c.InitiatedBy == user && c.DestinationUserId != "" select c;
                return cams.ToList();
            }
            catch (Exception t)
            {
                Logging.WriteLog(t.Message + ":" + t.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception","CAMForm");

            }
            return null;
        }

        //Retrieve all approved CAMS
        private List<CAM> retrieveApprovedCAMs()
        {
            try
            {
                string user1 = Session["Username"].ToString();
                var approvedCAMs = from ac in db.CAM where ac.InitiatedBy == user1  && ac.Status == "Approved" select ac;

                return approvedCAMs.ToList();

                //int pageSize = 10;

                //int pageNumber = (page ? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                //return View(db.CAM.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception s)
            {
                Logging.WriteLog(s.Message + ":" + s.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }
            return null;
        }

        public JsonResult GetApplicantsList()
        {
            //Retrieve Applicants brief for specific user
            string user = Session["Username"].ToString();
            try
            {
                List<ApplicantsBrief> applist = db.ApplicantsBrief.Where(s=> s.UpdateUserId == user).ToList();
                int recordCount = applist.Count;
                return Json(new { Result = "OK", Records = applist, TotalRecordCount = recordCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult AddApplicant(ApplicantsBrief applist)
        {
            try
            {
                applist.CreationDate = DateTime.Now.Date;
                applist.DateLastUpdated = DateTime.Now;
                applist.UpdateUserId = Session["Username"].ToString();

                // var region = db.Zone.Where(z => z.ZoneId == branch.ZoneId).SingleOrDefault();
                //applist.RegionId = region.RegionId;
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Some or all the fields are not valid. Confirm that the account number has numeric fields and the year of incorporation has 4 digits and it is not text." });
                }

                //Check if fields are empty
                else if (applist.AccountNumber == null)
                {

                    return Json(new {Result ="ERROR", Message = "The Account Number field is required."});
                }
                else if (applist.LegalIdentityName == null)
                {
                    return Json(new { Result = "ERROR", Message = "The Legal Identiy Name field is required." });

                }

                else if (applist.AccountNumber.ToString().Length < 10)
                {
                    return Json(new { Result = "ERROR", Message = "The Account Number fields must have 10 digits." });
                }

               
                
                
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Applicant Information " + applist.LegalIdentityName, UserId =  Session["Username"].ToString(),Designation=Session["maindesignation"].ToString()};
                db.Audit.Add(userAudit);
                db.ApplicantsBrief.Add(applist);
                db.SaveChanges();
                //var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = applist });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EditApplicant(ApplicantsBrief applist)
        {
            try
            {
                //region.CreationDate = DateTime.Now;
                applist.DateLastUpdated = DateTime.Now;
                applist.UpdateUserId = "Ayorinde";
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Some or all the fields are not valid. Confirm that the account number has numeric fields and the year of incorporation has 4 digits and it is not text." });
                }

                else if (applist.AccountNumber == null)
                {

                    return Json(new { Result = "ERROR", Message = "The Account Number field is required." });
                }
                else if (applist.LegalIdentityName == null)
                {
                    return Json(new { Result = "ERROR", Message = "The Legal Identiy Name field is required." });

                }

                else if (applist.AccountNumber.ToString().Length < 10)
                {
                    return Json(new { Result = "ERROR", Message = "The Account Number fields must have 10 digits." });
                }

                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Updated Applicant Information " + applist.LegalIdentityName, UserId =  Session["Username"].ToString() ,Designation=Session["maindesignation"].ToString()};
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
        public JsonResult DeleteApplicant(int ApplicantID)
        {
            try
            {
                Thread.Sleep(50);
                ApplicantsBrief applist = db.ApplicantsBrief.Find(ApplicantID);
                db.ApplicantsBrief.Remove(applist);
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Deleted Applicant" + applist.LegalIdentityName, UserId= Session["Username"].ToString(),Designation=Session["maindesignation"].ToString() };
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





        [HttpPost]
        public ActionResult Index(string customersearch, string CAMID)
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

                                  where ca.CAMId == bc.CAMId
                                    && ca.InitiatedBy == user
                                    && ca.Status == "Pending"
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


             //   int pageSize = 10;

             //   int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


             //   ViewBag.CAM = pendingcams1.ToPagedList(pageNumber, pageSize);

             //// ViewBag.CAM = pendingcams1;
               

            }
            catch (Exception ss)
            {
                Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }


          

            //return null;
        }
        

            

            else
            {

                /* --------------------------------------------------------------------------------------------------------------------------------      */
                try
                {
                    string user = Session["Username"].ToString();

                    //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                    var pendingCAMs = from ca in db.CAM
                                      // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
                                      join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
                                      join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
                                      join ft in db.FacilityType on bc.Facility equals ft.id

                                      where ca.CAMId == bc.CAMId
                                        && ca.InitiatedBy == user
                                        && ca.Status == "Pending"
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


                    ////int pageSize = 10;

                    //int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


                    //ViewBag.CAM = pendingcams1.ToPagedList(pageNumber, pageSize);

                    ViewBag.CAM = pendingcams1;

                }
                catch (Exception ss)
                {
                    Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                    //Redirect user to error page.
                    RedirectToAction("Exception", "CAMForm");
                }


           



                /*----------------------------------------------------------------------------------------------------------------------- */
                try
                {
                    
                        string user1 = Session["Username"].ToString();
                        var approvedCAMs = from ac in db.CAM where ac.InitiatedBy == user1  && ac.Status == "Approved" select ac;
                        if (approvedCAMs != null)
                        {
                            var approvedCAMss = approvedCAMs.OrderBy(s => s.CreationDate);
                            //int pageSize = 10;

                            //int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);



                            ViewBag.ApprovedCAMs = approvedCAMss;
                            //  return View(approvedCAMss.ToPagedList(pageNumber, pageSize));
                            return View(approvedCAMs);


                        }
                        // return approvedCAMs.ToList();

                        //int pageSize = 10;

                        //int pageNumber = (page ? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                        //return View(db.CAM.ToPagedList(pageNumber, pageSize));
                    }
                
                catch (Exception s)
                {
                    Logging.WriteLog(s.Message + ":" + s.StackTrace);
                    //Redirect user to error page.
                    RedirectToAction("Exception", "CAMForm");
                }

                //  return View();
                
            }
            return View();
        }
        //
        // GET: /Applicant/

        public ActionResult Index(int?page)
        {



            ///* -------------------------------------------Retreive all cams that are pending-------------------------------------------------------------------------------------      */
            try
            {
                string user = Session["Username"].ToString();

                //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                var pendingCAMs = from ca in db.CAM
                                  // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
                                  join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
                                  join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
                                  join ft in db.FacilityType on bc.Facility equals ft.id

                                  where ca.CAMId == bc.CAMId
                                    && ca.InitiatedBy == user
                                    && ca.Status == "Pending"

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

                var distinctcams1 = pendingCAMs.OrderByDescending(s => s.CreationDate);

                pendingcams2 = new List<PendingCamList>();
                foreach (var g in distinctcams1)
                {
                    pendingcams2.Add(new PendingCamList
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


                int pageSize = 10;

                int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


                ViewBag.CAM = pendingcams2.ToPagedList(pageNumber, pageSize);

               // ViewBag.CAM = pendingcams1;
                //return View(pendingcams1);

            }
            catch (Exception ss)
            {
                Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
                //Redirect user to error page.
                RedirectToAction("Exception", "CAMForm");
            }


         

           /*--------------------------------------------------APPROVED CAMS GET--------------------------------------------------------------------- */


            //try
            //{
            //    string user1 = Session["Username"].ToString();
            //    var approvedCAMs = from ac in db.CAM where ac.InitiatedBy == user1 && ac.Status == "Approved" select ac;
            //    if (approvedCAMs != null)
            //    {
            //        var approvedCAMss = approvedCAMs.OrderBy(s => s.CreationDate);
            //        //int pageSize = 10;

            //        //int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);



            //        ViewBag.ApprovedCAMs = approvedCAMss;
            //        //  return View(approvedCAMss.ToPagedList(pageNumber, pageSize));
            //        return View(approvedCAMs);


            //    }
            //    // return approvedCAMs.ToList();

            //    //int pageSize = 10;

            //    //int pageNumber = (page ? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

            //    //return View(db.CAM.ToPagedList(pageNumber, pageSize));
            //}
            //catch (Exception s)
            //{
            //    Logging.WriteLog(s.Message + ":" + s.StackTrace);
            //    //Redirect user to error page.
            //    RedirectToAction("Exception", "CAMForm");
            //}

          //  return null;
        //    return View();
            int pageSize2 = 10;

            int pageNumber2 = (page ?? 1); ViewBag.StartingNumber = (((pageNumber2 - 1) * pageSize2) + 1);


           return View(pendingcams2.ToPagedList(pageNumber2, pageSize2));
        }



       
        //
        // GET: /Applicant/Details/5

        public ActionResult Details(long id = 0)
        {
            ApplicantsBrief applicantsbrief = db.ApplicantsBrief.Find(id);
            if (applicantsbrief == null)
            {
                return HttpNotFound();
            }
            return View(applicantsbrief);
        }

        //
        // GET: /Applicant/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Applicant/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplicantsBrief applicantsbrief)
        {
            if (ModelState.IsValid)
            {
                db.ApplicantsBrief.Add(applicantsbrief);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicantsbrief);
        }

        //
        // GET: /Applicant/Edit/5

        public ActionResult Edit(long id = 0)
        {
            ApplicantsBrief applicantsbrief = db.ApplicantsBrief.Find(id);
            if (applicantsbrief == null)
            {
                return HttpNotFound();
            }
            return View(applicantsbrief);
        }

        //
        // POST: /Applicant/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicantsBrief applicantsbrief)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicantsbrief).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicantsbrief);
        }

        //
        // GET: /Applicant/Delete/5

        public ActionResult Delete(long id = 0)
        {
            ApplicantsBrief applicantsbrief = db.ApplicantsBrief.Find(id);
            if (applicantsbrief == null)
            {
                return HttpNotFound();
            }
            return View(applicantsbrief);
        }

        //
        // POST: /Applicant/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ApplicantsBrief applicantsbrief = db.ApplicantsBrief.Find(id);
            db.ApplicantsBrief.Remove(applicantsbrief);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        /*************************************************************************/
        //retrieve all cams to be used as template (JsonTemplate)
        //[HttpPost]
        //public JsonResult retrieveTemplateCams(string sstartdate, string eenddate)
        //{

        //    try
        //    {
        //        string user = Session["Username"].ToString();
        //        DateTime starttdate = DateTime.Parse(sstartdate);
        //        DateTime endddate = DateTime.Parse(eenddate);


        //        //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

        //        var pendingCAMs = from ca in db.CAM
        //                          // join ab in db.CreditRequestSummary on ca.CAMId equals ab.CAMId
        //                          join bc in db.CreditRequestSummary on ca.CAMId equals bc.CAMId
        //                          join ba in db.ApplicantsBrief on ca.ApplicantID equals ba.ApplicantID
        //                          join ft in db.FacilityType on bc.Facility equals ft.id


        //                          where bc.Facility == ft.id
        //                              // && ca.ApplicantID == ba.ApplicantID
        //                          && ca.ApplicantID == bc.ApplicantID
        //                              // && ca.CAMId == ab.CAMId
        //                        && ca.CAMId == bc.CAMId
        //                        && ca.CreationDate >= starttdate && ca.CreationDate <= endddate
        //                          && ca.InitiatedBy == user




        //                          //   && ca.DestinationUserId != ""

        //                          select new
        //                          {
        //                              ca.CAMId,
        //                              ca.InitiatedBy,
        //                              ca.DestinationUserId,
        //                              ba.LegalIdentityName,
        //                              ft.description,
        //                              ca.TotalFacilityAmount,
        //                              ca.CreationDate,
        //                              ca.DateLastUpdated,
        //                              ca.Status
        //                          };
        //        var distinctcams = pendingCAMs.Distinct();
        //        var distinctcams1 = distinctcams.OrderBy(s => s.CreationDate);

        //        List<PendingCamList> pendingcams2 = new List<PendingCamList>();
        //        foreach (var g in distinctcams1)
        //        {
        //            pendingcams2.Add(new PendingCamList
        //            {
        //                camid = g.CAMId,
        //                initiatedby = g.InitiatedBy,
        //                destinationuser = g.DestinationUserId,
        //                customername = g.LegalIdentityName,
        //                facilitytype = g.description,
        //                facilityamount = g.TotalFacilityAmount.ToString(),
        //                dateinitiated = DateTime.Parse(g.CreationDate.ToString()),
        //                datelastupdated = DateTime.Parse(g.DateLastUpdated.ToString()),
        //                status = g.Status

        //            });

        //        }


        //        //int pageSize = 10;

        //        //int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);


        //       // ViewBag.TemplateCAM = pendingcams2;

        //        return Json(new { Result = "OK" },ViewBag.TemplateCAM=pendingcams2);
        //    }
        //    catch (Exception ss)
        //    {
        //        Logging.WriteLog(ss.Message + ":" + ss.StackTrace);
        //        //Redirect user to error page.
        //        RedirectToAction("Exception", "CAMForm");
        //    }

        //    return null;
        //}

        /************************************************************************/
    }
}