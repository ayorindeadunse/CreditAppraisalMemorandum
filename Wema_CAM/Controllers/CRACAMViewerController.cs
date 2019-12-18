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
    public class CRACAMViewerController : Controller
    {
        // List<CAM> outstandingCAMsList;
        string outstandingCAMsList = null;
        string craCAM = null;
        string userfunction = null;
        decimal userapprovallimit = 0;
        List<Comment> comm = null;
        string useremailaddress = null;
        string crrtext = null;
        string crrtext1 = null;
        long appid = 0;
        long cid = 0;
        DateTime startdate3;
        DateTime enddate3;
        string mainbranch = null;

        private Wema_CAMEntities db = new Wema_CAMEntities();

        //
        // GET: /CRACAMViewer/

        public ActionResult Index(int? page)
        {
            //int? page = 1;
            //  return View(db.CAM.ToList());

            ////Return all the CAMs where the DestinationUserId equals the log on UserId
         //   outstandingCAMsList = Session["Username"].ToString();
            craCAM = Session["UserFunction"].ToString();

            if (craCAM == "CRA")
            {
                //var outstandingCAMs = from c in db.CAM where c.Status == "Pending" && c.InitiatedBy != c.DestinationUserId select c;
                //if (outstandingCAMs != null)
                //{
                //    var outstandingcams = outstandingCAMs.OrderBy(s => s.CreationDate);
                //    int pageSize = 10;

                //    int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                //    return View(outstandingcams.ToPagedList(pageNumber, pageSize));


                //}

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
                                          // && ca.DestinationUserId == user
                                       && ca.DestinationUserId == user
                                        && ca.Status == "Pending"



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

                    ViewBag.CAM = pendingcams1;

                    int pageSize = 10;

                    int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                    return View(pendingcams1.ToPagedList(pageNumber, pageSize));

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

        [HttpPost]
        public ActionResult Index(string customersearch, int? page)
        {
            //int? page = 1;
            //  return View(db.CAM.ToList());

            ////Return all the CAMs where the DestinationUserId equals the log on UserId
            //   outstandingCAMsList = Session["Username"].ToString();
            craCAM = Session["UserFunction"].ToString();

            if (craCAM == "CRA")
            {
                //var outstandingCAMs = from c in db.CAM where c.Status == "Pending" && c.InitiatedBy != c.DestinationUserId select c;
                //if (outstandingCAMs != null)
                //{
                //    var outstandingcams = outstandingCAMs.OrderBy(s => s.CreationDate);
                //    int pageSize = 10;

                //    int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                //    return View(outstandingcams.ToPagedList(pageNumber, pageSize));


                //}

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
                                          // && ca.DestinationUserId == user
                                       && ca.DestinationUserId == user
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

                    ViewBag.CAM = pendingcams1;

                    int pageSize = 10;

                    int pageNumber = (page ?? 1); ViewBag.StartingNumber = (((pageNumber - 1) * pageSize) + 1);

                    return View(pendingcams1.ToPagedList(pageNumber, pageSize));

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
        //[HttpPost]
        public ActionResult Download(int id = 0)
        {
            try
            {

                var uploads = (from u in db.fileinformation
                               where u.fileid == id
                               select u.filepath).FirstOrDefault();


                if (uploads != null)
                {
                    // string folder = Path.GetFullPath(uploads);
                    string file = Server.MapPath(@"~/App_Data_Old\") + uploads.ToString().Replace("/", @"\");
                    string fileextension = file.Split(new char[] { '.' })[2];

                    if (fileextension.Equals("pdf"))
                    {
                        return File(file, "application/pdf");
                    }
                    else if (fileextension.Equals("xls") || fileextension.Equals("xslx"))
                    {
                        return File(file, "application/x-msexcel");
                    }
                    else if (fileextension.Equals("doc") || fileextension.Equals("docx"))
                    {
                        return File(file, "application/msword");
                    }

                    else
                    {
                        ViewBag.FileNotSupported = "The selected file cannot be viewed.";

                        //  return View("Details");
                    }
                }

            }
            catch (Exception ext)
            {
                Logging.WriteLog(ext.Message + ":" + ext.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");
            }
            return null;
        }



        //Insert comment from user into database and approve
        [HttpPost]
        public JsonResult insertComments(string comment)
        {

            try
            {

                //insert comment into database 
                Comment comme = new Comment();
                comme.CAMId = long.Parse(Session["CamId"].ToString());
                comme.Username = Session["Username"].ToString();
                comme.Designation = Session["maindesignation"].ToString();
                comme.Comment1 = comment;
                comme.CommentDate = DateTime.Now;
                comme.EditDate = DateTime.Now;

                db.Comment.Add(comme);

                //Insert into Audit table
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "Added Comment " + comment, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                db.SaveChanges();

                return Json(new { Result = "OK", Message = "Comment added successfully!", Feedback = retrieveCommentDetails() });
            }

            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //Retrieve other transactions for user
        [HttpPost]
        public JsonResult retrievependinganddeclined(string status1, string startdate2, string enddate2)
        {
            try
            {
                if (status1 != "" && startdate2 == "" && enddate2 == "")
                {
                    return Json(new { Result = "OK", Message = "Query executed successfully!", Feedback = retrievependinganddeclinedrequests(status1) });
                }
                else if (status1 != "" && startdate2 != "" && enddate2 != "")
                {
                    return Json(new { Result = "OK", Message = "Query executed successfully!", Feedback = retrievependinganddeclinedrequests(status1, startdate2, enddate2) });
                }
            }
            catch (Exception csr)
            {
                return Json(new { Result = "ERROR", Message = csr.Message });
            }
            return null;
        }

        public string retrievependinganddeclinedrequests(string status1)
        {
            string designationofuser = null;
            string branchofuser = null;
            string zoneofuser = null;
            string regionofuser = null;

            try
            {

                //Retreve the branch, zone or region of logged on user.
                if (Session["UserFunction"] != null)
                {
                    designationofuser = Session["UserFunction"].ToString();

                    if (designationofuser == "BDM")
                    {
                        branchofuser = Session["BRANCH"].ToString();

                        var camstreatedinbranch = from cb in db.CAM where cb.BranchProcessedFrom == branchofuser && cb.Status == status1 select cb;
                        if (camstreatedinbranch != null)
                        {
                            var maincamstreatedinbranch = camstreatedinbranch.ToList();
                            // ViewBag.branchCAMs = maincamstreatedinbranch;

                            // return Json(new { Result = "OK", Message = "Query executed successfully!", Feedback = retrieveCommentDetails() });


                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinbranch)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;

                        }
                    }

                    else if (designationofuser == "ZM")
                    {
                        zoneofuser = Session["ZONE"].ToString();
                        var camstreatedinzone = from cz in db.CAM where cz.ZoneProcessedFrom == zoneofuser && cz.Status == status1 select cz;
                        if (camstreatedinzone != null)
                        {
                            var maincamstreatedinzone = camstreatedinzone.ToList();

                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinzone)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                            // ViewBag.zoneCAMs = maincamstreatedinzone;
                        }
                    }
                    else if (designationofuser == "RE")
                    {
                        regionofuser = Session["region"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.RegionProcessedFrom == regionofuser && cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                    //Unit Head of Corporate Banking and Group Head of Corporate Banking should see which credits?
                    else if (designationofuser == "UH")
                    {
                        branchofuser = Session["BRANCH"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.BranchProcessedFrom == branchofuser && cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {

                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                    else if (designationofuser == "CRO")
                    {
                        branchofuser = Session["BRANCH"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                    else if (designationofuser == "CRA")
                    {
                        branchofuser = Session["BRANCH"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                    else if (designationofuser == "ED")
                    {
                        branchofuser = Session["BRANCH"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                    else if (designationofuser == "MD/CEO")
                    {
                        branchofuser = Session["BRANCH"].ToString();
                        var camstreatedinregion = from cr in db.CAM where cr.Status == status1 select cr;
                        if (camstreatedinregion != null)
                        {
                            var maincamstreatedinregion = camstreatedinregion.ToList();
                            //   ViewBag.regionCAMs = maincamstreatedinregion;
                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinregion)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                        }
                    }

                }


            }


            catch (Exception ext)
            {

                // return Json(new { Result = "ERROR", Message = ext.Message });
            }
            return null;
        }

        public string retrievependinganddeclinedrequests(string status1, string startdate2, string enddate2)
        {
            string designationofuser = null;
            string branchofuser = null;
            string zoneofuser = null;
            string regionofuser = null;

            try
            {

                //Retreve the branch, zone or region of logged on user.
                if (Session["UserFunction"] != null)
                {
                    designationofuser = Session["UserFunction"].ToString();
                    startdate3 = DateTime.Parse(startdate2.ToString());
                    enddate3 = DateTime.Parse(enddate2.ToString());

                    if (designationofuser == "BDM")
                    {
                        branchofuser = Session["BRANCH"].ToString();

                        var camstreatedinbranch = from cb in db.CAM where cb.BranchProcessedFrom == branchofuser && cb.Status == status1 && cb.CreationDate >= startdate3 && cb.CreationDate <= enddate3 select cb;
                        if (camstreatedinbranch != null)
                        {
                            var maincamstreatedinbranch = camstreatedinbranch.ToList();
                            // ViewBag.branchCAMs = maincamstreatedinbranch;

                            // return Json(new { Result = "OK", Message = "Query executed successfully!", Feedback = retrieveCommentDetails() });


                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinbranch)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;

                        }
                    }

                    else if (designationofuser == "ZM")
                    {
                        zoneofuser = Session["ZONE"].ToString();
                        var camstreatedinzone = from cz in db.CAM where cz.ZoneProcessedFrom == zoneofuser && cz.Status == status1 && cz.CreationDate >= startdate3 && cz.CreationDate <= enddate3 select cz;
                        if (camstreatedinzone != null)
                        {
                            var maincamstreatedinzone = camstreatedinzone.ToList();

                            string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                            foreach (var f in maincamstreatedinzone)
                            {
                                tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                            }
                            //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                            //return cre;
                            tableStructure = tableStructure + "</table>";


                            return tableStructure;
                            // ViewBag.zoneCAMs = maincamstreatedinzone;
                        }
                        else if (designationofuser == "RE")
                        {
                            regionofuser = Session["region"].ToString();
                            var camstreatedinregion = from cr in db.CAM where cr.RegionProcessedFrom == regionofuser && cr.Status == status1 && cr.CreationDate >= startdate3 && cr.CreationDate <= enddate3 select cr;
                            if (camstreatedinregion != null)
                            {
                                var maincamstreatedinregion = camstreatedinregion.ToList();
                                //   ViewBag.regionCAMs = maincamstreatedinregion;
                                string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                                foreach (var f in maincamstreatedinregion)
                                {
                                    tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                                }
                                //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                                //return cre;
                                tableStructure = tableStructure + "</table>";


                                return tableStructure;
                            }
                        }

                        else if (designationofuser == "UH")
                        {
                            branchofuser = Session["BRANCH"].ToString();
                            var camstreatedinregion = from cr in db.CAM where cr.RegionProcessedFrom == branchofuser && cr.Status == status1 select cr;
                            if (camstreatedinregion != null)
                            {
                                var maincamstreatedinregion = camstreatedinregion.ToList();
                                //   ViewBag.regionCAMs = maincamstreatedinregion;
                                string tableStructure = "<table class=\"tblboda1\"><tr><th>Cam Id</th><th>Branch Processed</th><th>Zone Processed</th><th>Region Processed</th><th>Status</th><th>Initiated By</th><th>Current Location</th><th>Date Created</th></tr>";
                                foreach (var f in maincamstreatedinregion)
                                {
                                    tableStructure = tableStructure + "<tr><td>" + f.CAMId + "</td>" + "<td>" + f.BranchProcessedFrom + "</td>" + "<td>" + f.ZoneProcessedFrom + "</td>" + "<td>" + f.RegionProcessedFrom + "</td>" + "<td>" + f.Status + "</td>" + "<td>" + f.InitiatedBy + "</td>" + "<td>" + f.DestinationUserId + "</td>" + "<td>" + f.CreationDate + "</td>" + "</tr>";
                                }
                                //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                                //return cre;
                                tableStructure = tableStructure + "</table>";


                                return tableStructure;
                            }
                        }



                    }


                }

            }
            catch (Exception ext)
            {

                // return Json(new { Result = "ERROR", Message = ext.Message });
            }
            return null;
        }

        //Insert comment from user into database and decline
        [HttpPost]
        public JsonResult declineTransaction(string comment, long CAMId)
        {
            Session["CamId"] = CAMId;
            try
            {
                userfunction = Session["UserFunction"].ToString();

                //Decline CAM
                CAM cam = db.CAM.Find(CAMId);
                cam.Status = "Declined";
                cam.DestinationUserId = Session["Username"].ToString();

                //Add Audit information and save changes.
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + "declined by " + Session["Username"].ToString(), UserId = Session["Username"].ToString() };
                db.Audit.Add(userAudit);


                //insert comment into database still
                Comment commerr = new Comment();
                commerr.CAMId = int.Parse(Session["CamId"].ToString());
                commerr.Username = Session["Username"].ToString();
                commerr.Comment1 = comment;
                commerr.CommentDate = DateTime.Now;
                commerr.EditDate = DateTime.Now;

                db.Comment.Add(commerr);


                db.SaveChanges();
                //Redirect user to page.


                return Json(new { Result = "OK" });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //Insert comment from user into database and move transaction to next user on queue

        public JsonResult moveTransaction(string comment, long CAMId, string NextLevelSupervisor)
        {

            try
            {
                userfunction = Session["UserFunction"].ToString();

                //Move CAM
                CAM cam = db.CAM.Find(CAMId);
                cam.Status = "Pending";
                cam.DestinationUserId = Session["Username"].ToString();
                cam.DestinationUserId = NextLevelSupervisor;

                //Add Audit information and save changes.
                Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + " is now pending with " + NextLevelSupervisor, UserId = Session["Username"].ToString() };
                db.Audit.Add(userAudit);


                //insert comment into database still
                Comment commerr = new Comment();
                commerr.CAMId = int.Parse(Session["CamId"].ToString());
                commerr.Username = Session["Username"].ToString();
                commerr.Comment1 = comment;
                commerr.CommentDate = DateTime.Now;
                commerr.EditDate = DateTime.Now;

                db.Comment.Add(commerr);


                db.SaveChanges();
                //Redirect user to page.


                return Json(new { Result = "OK" });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        //
        // GET: /CRACAMViewer/Details/5

        public ActionResult Details(long id = 0)
        {


            //Get the Viewbag message
            CAM cam = db.CAM.Find(id);
            decimal TotalFacilityAmount;
            TotalFacilityAmount = decimal.Parse(cam.TotalFacilityAmount.ToString());


            //Retrieve Originating Branch

            var origbranch = from b in db.CAM where b.CAMId == id select b;
            if (origbranch != null)
            {
                foreach (var bb in origbranch)
                {
                    mainbranch = bb.BranchProcessedFrom;
                }
            }

            ViewBag.MainBranch = mainbranch;


            // cam.TotalFacilityAmount = TotalFacilityAmount;

            //Retrieve Next Level Supervisor
            Session["applicantid"] = id;
            //  ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString(),TotalFacilityAmount), "Username", "Username");


            Session["CamId"] = id;
            ViewBag.Comment = retrieveComments();
            ViewBag.SupportingDocuments = retrieveSupportingDocuments();


            if (cam == null)
            {
                return HttpNotFound();
            }

            // appid = long.Parse(Session["appid"].ToString());
            appid = long.Parse(cam.ApplicantID.ToString());


            var credittype = from ct in db.ApplicantsBrief where ct.ApplicantID == appid select ct;

            if (credittype != null)
            {
                foreach (var crtype in credittype)
                {
                    long ApplicantType = long.Parse(crtype.ApplicantType.ToString());

                    var crtyptext = from ctt in db.ApplicantType where ctt.id == ApplicantType select ctt;

                    if (crtyptext != null)
                    {
                        foreach (var crr in crtyptext)
                        {
                            crrtext = crr.descritpion;
                        }
                    }
                }
            }

            //Add the credit type to the session
            Session["creditypte"] = crrtext;


            crrtext1 = Session["creditypte"].ToString();

            if (crrtext1 == "Commercial")
            {
                //  ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                //  ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                ViewBag.NextLevelList = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString(), TotalFacilityAmount), "Username", "Username");
            }

            else if (crrtext1 == "Corporate")
            {
                ViewBag.NextLevelList = new SelectList(CamHelper.GetNextCorporateUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
            }




            //run a query to retrieve information from the ApplicantsBrief table and Credit Request Summary Table and put inside the Viewbag.
            try
            {
                //var applicantsbrief = from ab in db.ApplicantsBrief where ab.ApplicantID == appid select ab;

                var applicantsbrief = from ab in db.ApplicantsBrief
                                      join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                      join ef in db.IndustryType on ab.Industry equals ef.id
                                      where ab.ApplicantType == cd.id & ab.Industry == ef.id
                                      && ab.ApplicantID == appid
                                      select new
                                      {
                                          cd.descritpion,
                                          ab.LegalIdentityName,
                                          ab.YearOfIncorporation,
                                          ab.RCNumber,
                                          ab.BusinessAddress,
                                          ab.NameOfAuditors,
                                          ab.NatureOfBusiness,
                                          ef.industrydescription,
                                          ab.ProductRange,
                                          ab.MajorClients
                                      };


                List<appbrief> appbrief = new List<appbrief>();
                foreach (var s in applicantsbrief)
                {
                    appbrief.Add(new appbrief
                    {
                        apptype = s.descritpion,
                        legalidentityname = s.LegalIdentityName,
                        yearofincorporation = s.YearOfIncorporation,
                        rcnumber = s.RCNumber,
                        businessaddress = s.BusinessAddress,
                        nameofauditors = s.NameOfAuditors,
                        natureofbusiness = s.NatureOfBusiness,
                        industry = s.industrydescription,
                        productrange = s.ProductRange,
                        majorclients = s.MajorClients
                    });
                }


                ViewBag.ApplicantsBriefInfo = appbrief;

                if (Session["CamId"] != null)
                {
                    cid = long.Parse(Session["CamId"].ToString());
                }

                var creditrequest = from cr in db.CreditRequestSummary
                                    join ab in db.FacilityType on cr.Facility equals ab.id
                                    join dd in db.ApplicationType on cr.ApplicationType equals dd.id


                                    where cr.Facility == ab.id
                                    && cr.ApplicantID == appid
                                    && cr.ApplicationType == dd.id
                                    && cr.CAMId == cid

                                    select new
                                    {
                                        cr.CAMId,
                                        ab.description,
                                        cr.FacilityAmount,
                                        dd.applicationdescription,
                                        cr.Purpose,
                                        cr.Tenor,
                                        cr.InterestRate,
                                        cr.ProcessingFee,
                                        cr.ManagementFee,
                                        cr.LCCommission,
                                        cr.OtherCharges,
                                        cr.COTRate,
                                        cr.SourceOfRepayment,
                                        cr.ModeOfRepayment,
                                        cr.Security,
                                        cr.Guarantee
                                    };
                List<creditrequest> creditrequestt = new List<creditrequest>();
                foreach (var t in creditrequest)
                {
                    creditrequestt.Add(new creditrequest
                    {
                        camid = long.Parse(t.CAMId.ToString()),
                        facilitytype = t.description,
                        facilityamount = t.FacilityAmount,
                        applicanttype = t.applicationdescription,
                        purpose = t.Purpose,
                        tenor = t.Tenor,
                        interestrate = t.InterestRate,
                        processingfee = t.ProcessingFee,
                        managementfee = t.ManagementFee,
                        lccommission = t.LCCommission,
                        othercharges = t.OtherCharges,
                        cotrate = t.COTRate,
                        sourceofrepayment = t.SourceOfRepayment,
                        modeofrepayment = t.ModeOfRepayment,
                        security = t.Security,
                        guarantee = t.Guarantee
                    });
                }

                ViewBag.CreditRequestInfo = creditrequestt;
            }
            catch (Exception es)
            {
                Logging.WriteLog(es.Message + ":" + es.StackTrace);
                //Redirect user to error page.

                return RedirectToAction("Exception", "CAMForm");
            }

            return View(cam);
        }

        [HttpPost]
        public ActionResult Details(string NextLevelSupervisor, string SelectAction, decimal TotalFacilityAmount, long CAMId)
        {
            //APPROVE TRANSACTION
            if (SelectAction == "Approve")
            {
                try
                {
                    userfunction = Session["UserFunction"].ToString();
                    //Check if user is in role to approve Credit and if he/she is up to the approved limit.
                    var approvallimits = from b in db.ApprovalLimit where b.Grade == userfunction select b;

                    if (approvallimits != null)
                    {
                        var approvallimit = approvallimits.ToList();

                        foreach (var applimit in approvallimit)
                        {
                            userapprovallimit = decimal.Parse(applimit.ApprovalLimit1.ToString());

                        }

                        if (userapprovallimit == 0 || userapprovallimit < TotalFacilityAmount)
                        {
                            return RedirectToAction("NotAllowed");
                        }

                        else
                        {
                            //Approve credit and close CAM
                            CAM cam = db.CAM.Find(CAMId);
                            //  cam.DestinationUserId = "";
                            cam.DestinationUserId = Session["Username"].ToString();
                            cam.Status = "Approved";
                            //Add Audit info
                            Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + "approved by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };

                            db.Audit.Add(userAudit);

                            //Insert into the ActionAudit table  
                            CAM camm = db.CAM.Find(CAMId);

                            ActionAudit aa = new ActionAudit { CAMId = CAMId, Action = "Approved by " + Session["Username"].ToString(), FromUserId = "From " + camm.InitiatedBy, Designation = Session["maindesignation"].ToString(), DateActionPerformed = DateTime.Now };
                            db.ActionAudit.Add(aa);
                            db.SaveChanges();

                            //Retrieve Next Level Supervisor e-mail address
                            var emails = from ee in db.CAM_User where ee.Username == camm.InitiatedBy select ee;
                            if (emails != null)
                            {
                                var emailaddress = emails.ToList();
                                foreach (var useremail in emailaddress)
                                {
                                    useremailaddress = useremail.EmailAddress;
                                }

                            }
                            //Send e-mail to user

                            new CamHelper().approvedTransactionMail(useremailaddress, camm.InitiatedBy, camm.CAMId);
                            //Redirect user to page.

                            return RedirectToAction("Index");
                        }
                    }

                    // return Json(new { Result = "OK" });
                }
                catch (Exception ex)
                {
                    //return Json(new { Result = "ERROR", Message = ex.Message });
                    Logging.WriteLog(ex.Message + ":" + ex.StackTrace);
                    return RedirectToAction("Exception", "CAMForm");

                }
            }

                //DECLINE TRANSACTION
            else if (SelectAction == "Decline")
            {
                try
                {
                    userfunction = Session["UserFunction"].ToString();

                    //Decline CAM
                    CAM cam = db.CAM.Find(CAMId);
                    cam.Status = "Declined";
                    cam.DestinationUserId = "";

                    //Add Audit information and save changes.
                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + "declined by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    db.Audit.Add(userAudit);

                    //Insert into ActionAudit table
                    CAM camm = db.CAM.Find(CAMId);
                    ActionAudit aa = new ActionAudit { CAMId = CAMId, Action = "Declined by " + Session["Username"].ToString(), FromUserId = "From " + camm.InitiatedBy, Designation = Session["maindesignation"].ToString(), DateActionPerformed = DateTime.Now };
                    db.ActionAudit.Add(aa);
                    db.SaveChanges();

                    //Retrieve Next Level Supervisor e-mail address
                    var emails = from ee in db.CAM_User where ee.Username == camm.InitiatedBy select ee;
                    if (emails != null)
                    {
                        var emailaddress = emails.ToList();
                        foreach (var useremail in emailaddress)
                        {
                            useremailaddress = useremail.EmailAddress;
                        }

                    }
                    //Send e-mail to user

                    new CamHelper().declineTransactionMail(useremailaddress, camm.InitiatedBy);
                    //Redirect user to page.
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message + ":" + ex.StackTrace);
                    //Redirect user to error page.
                    return RedirectToAction("Exception", "CAMForm");
                }

            }

            //SEND TRANSACTION BACK TO USER

            else if (SelectAction == "More Information Required")
            {

                try
                {
                    CAM cammm = db.CAM.Find(CAMId);
                    userfunction = Session["UserFunction"].ToString();

                    //Decline CAM
                    CAM cam = db.CAM.Find(CAMId);
                    cam.Status = "Pending";
                    //cam.DestinationUserId = "";
                    cam.DestinationUserId = cammm.InitiatedBy;

                    //Add Audit information and save changes.
                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + "reviewed by " + Session["Username"].ToString(), UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    db.Audit.Add(userAudit);

                    //Insert into ActionAudit table
                    CAM camm = db.CAM.Find(CAMId);

                    ActionAudit aa = new ActionAudit { CAMId = CAMId, Action = "reviewed by " + Session["Username"].ToString(), FromUserId = "From " + camm.InitiatedBy, Designation = Session["maindesignation"].ToString(), DateActionPerformed = DateTime.Now };
                    db.ActionAudit.Add(aa);
                    db.SaveChanges();

                    //Retrieve Next Level Supervisor e-mail address
                    var emails = from ee in db.CAM_User where ee.Username == camm.InitiatedBy select ee;
                    if (emails != null)
                    {
                        var emailaddress = emails.ToList();
                        foreach (var useremail in emailaddress)
                        {
                            useremailaddress = useremail.EmailAddress;
                        }

                    }
                    //Send e-mail to user

                    new CamHelper().reviewTransactionMail(useremailaddress, camm.InitiatedBy);
                    //Redirect user to page.
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message + ":" + ex.StackTrace);
                    //Redirect user to error page.
                    return RedirectToAction("Exception", "CAMForm");
                }

            }

//MOVE TRANSACTION TO NEXT USER ON THE QUEUE
            else if (SelectAction == "Move")
            {

                try
                {
                    userfunction = Session["UserFunction"].ToString();

                    //Decline CAM
                    CAM cam = db.CAM.Find(CAMId);
                    cam.Status = "Pending";
                    cam.DestinationUserId = NextLevelSupervisor;

                    //Add Audit information and save changes.
                    Audit userAudit = new Audit { ActivityDate = DateTime.Now, Description = "CAM " + CAMId + "moved by " + Session["Username"].ToString() + " to " + NextLevelSupervisor, UserId = Session["Username"].ToString(), Designation = Session["maindesignation"].ToString() };
                    db.Audit.Add(userAudit);

                    //Insert into ActionAudit table
                    CAM camm = db.CAM.Find(CAMId);

                    ActionAudit aa = new ActionAudit { CAMId = CAMId, Action = "moved by " + Session["Username"].ToString(), FromUserId = "From " + camm.InitiatedBy, Designation = Session["maindesignation"].ToString(), DestinationUserId = "To " + NextLevelSupervisor, DateActionPerformed = DateTime.Now };
                    db.ActionAudit.Add(aa);
                    db.SaveChanges();

                    //Retrieve Next Level Supervisor e-mail address
                    var emails = from ee in db.CAM_User where ee.Username == NextLevelSupervisor select ee;
                    if (emails != null)
                    {
                        var emailaddress = emails.ToList();
                        foreach (var useremail in emailaddress)
                        {
                            useremailaddress = useremail.EmailAddress;
                        }

                    }
                    //Send e-mail to user

                    new CamHelper().alertNextOnQueue(NextLevelSupervisor, useremailaddress, camm.InitiatedBy);
                    //Redirect user to page.
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message + ":" + ex.StackTrace);
                    //Redirect user to error page.
                    return RedirectToAction("Exception", "CAMForm");
                }

            }


            return View();

        }



        //
        // GET: /CRACAMViewer/Create

        //Retrieve supporting documents
        private List<fileinformation> retrieveSupportingDocuments()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var sd = from s in db.fileinformation where s.CAMId == camId select s;
            return sd.ToList();
        }


        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CRACAMViewer/Create

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
        // GET: /CRACAMViewer/Edit/5

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
        // POST: /CRACAMViewer/Edit/5
        public ActionResult NotAllowed()
        {
            return View();

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
           [HttpPost, ValidateInput(false)]
        public ActionResult Edit(CAM cam)
        {
          //  cam.ApplicantID = long.Parse(ApplicantID.ToString());
            cam.DateLastUpdated = DateTime.Now;
            cam.UpdateUserId = Session["Username"].ToString();
            cam.Status = "Pending";
          //  cam.InitiatedBy = Session["Username"].ToString();
            cam.CreationDate = DateTime.Now;
            cam.DestinationUserId = Session["Username"].ToString();
         //   cam.RegionProcessedFrom = Session["Region"].ToString();
         //   cam.ZoneProcessedFrom = Session["ZONE"].ToString();
         //   cam.BranchProcessedFrom = Session["BRANCH"].ToString();

            if (ModelState.IsValid)
            {
                db.Entry(cam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cam);
        }

        //
        // GET: /CRACAMViewer/Delete/5

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
        // POST: /CRACAMViewer/Delete/5

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

        private List<Comment> retrieveComments()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;
            return com.ToList();
        }

        private string retrieveCommentDetails()
        {
            int camId = int.Parse(Session["CamId"].ToString());
            var com = from c in db.Comment where c.CAMId == camId select c;

            string tableStructure = "<table class=\"tblboda\"><tr><th>Username</th><th>Designation</th><th>Comment</th><th>Date</th></tr>";
            foreach (var f in com)
            {
                tableStructure = tableStructure + "<tr><td>" + f.Username + "</td>" + "<td>" + f.Designation + "</td>" + "<td>" + f.Comment1 + "</td>" + "<td>" + f.CommentDate + "</td></tr>";
            }
            tableStructure = tableStructure + "</table>";
            return tableStructure;

        }
    }


}