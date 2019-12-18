using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wema_CAM.Models;

namespace Wema_CAM.Controllers
{
   
    public class CAMReportsController : Controller
    {
        List<string> regiontypes = null;
        List<string> statustypes = null;
        List<string> FacilityTypes = null;
        List<string> ApplicationType = null;
        List<string> IndustryType = null;
        List<appbrief> app = null;
        long appid = 0;
        long facid = 0;
        long appppid = 0;
        
        
        List<creditrequestobjects> cre = null;

        private Wema_CAMEntities db = new Wema_CAMEntities();

        //
        // GET: /CAMReports/

        public ActionResult Index()
        {
            try
            {
                //Region dropdown
                regiontypes = new List<string>();
                statustypes = new List<string>();
                FacilityTypes = new List<string>();
                ApplicationType = new List<string>();
                IndustryType = new List<string>();

                var reg = from rr in db.Region select rr;
                if (reg != null)
                {
                    var regions = reg.ToList();
                    foreach (var regionss in regions)
                    {
                        string regionname = regionss.RegionName;
                        regiontypes.Add(regionname);
                    }
                }

                //Status dropdown
                var status = from ss in db.State select ss;
                if (status != null)
                {
                    var stattus = status.ToList();
                    foreach (var stattuss in stattus)
                    {
                        string statusname = stattuss.description;
                        statustypes.Add(statusname);
                    }
                }

                //Facility Types dropdown
                var facilitytypes = from ft in db.FacilityType select ft;
                if (facilitytypes != null)
                {
                    var facillitytypes = facilitytypes.ToList();
                    foreach (var facillitytypess in facillitytypes)
                    {
                        string factypes = facillitytypess.description;
                        FacilityTypes.Add(factypes);
                    }

                }

                //Application Types dropdown
                var applicationtypes = from at in db.ApplicationType select at;
                if (applicationtypes != null)
                {
                    var applicationntypes = applicationtypes.ToList();
                    foreach (var appllicationtypes in applicationntypes)
                    {
                        string apptypes = appllicationtypes.applicationdescription;
                        ApplicationType.Add(apptypes);
                    }

                }

                //Industry Types dropdown
                var industrytypes = from it in db.IndustryType select it;
                if (industrytypes != null)
                {
                    var industryytypes = industrytypes.ToList();
                    foreach (var industryytyype in industryytypes)
                    {
                        string indtypes = industryytyype.industrydescription;
                        IndustryType.Add(indtypes);
                    }

                }
                //ViewBag.Regions = new SelectList(CamHelper.GetNextLevelUserList(Session["UserFunction"].ToString(), Session["BranchId"].ToString()), "Username", "Username");
                ViewBag.Regions = new SelectList(regiontypes);
                ViewBag.Status = new SelectList(statustypes);
                ViewBag.FacilityType = new SelectList(FacilityTypes);
                ViewBag.ApplicationType = new SelectList(ApplicationType);
                ViewBag.IndustryType = new SelectList(IndustryType);


                return View(db.CAM.ToList());
            }
            catch (Exception r)
            {
                Logging.WriteLog(r.Message + ":" + r.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");
            }

          
            
        }


         //Json method to handle other reports.
        [HttpPost]
        public JsonResult GenerateOtherReports(string customer, string FacilityType, string facilitylimit,
            string tenor, string ApplicationType, string security, string IndustryType,string StatusType, string interestrate, string processingfee, string startdate1, string enddate1)
        {
            try
            {
                if (customer != "")
                {
                    Session["CUSTOMER"] = customer;
                }
                if (FacilityType != "")
                {
                    Session["FACILITYTYPE"] = FacilityType;
                }
                if (facilitylimit != "")
                {
                    Session["FACILITYLIMIT"] = facilitylimit;
                }
                if (tenor != "")
                {
                    Session["TENOR"] = tenor;
                }
                if (ApplicationType != "")
                {
                    Session["APPLICATIONTYPE"] = ApplicationType;
                }
                if (security != "")
                {
                    Session["SECURITY"] = security;
                }
                if (IndustryType != "")
                {
                    Session["INDUSTRYTYPE"] = IndustryType;
                }
                if (StatusType != "")
                {
                    Session["STATUS"] = StatusType;
                }
                if (interestrate != "")
                {
                    Session["INTERESTRATE"] = interestrate;
                }
                if (processingfee != "")
                {
                    Session["PROCESSINGFEE"] = processingfee;
                }
                if (startdate1 != "")
                {
                    Session["STARTDATE1"] = startdate1;
                }
                if (enddate1 != "")
                {
                    Session["ENDDATE1"] = enddate1;
                }

                //Return the number of credit requests(Renewals, Extensions etc)
                //from a customer in a given period.

                if (customer != "" && ApplicationType != "" && startdate1 != "" && enddate1 != ""
                    && FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType == ""
                    && StatusType == "" && interestrate == "" && processingfee == "")
                {
                    //  ViewBag.CreditRequests = returnCreditRequestsByDate();
                    //  }
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnCreditRequestsByDate() });

                }

                //Returns the facility types in a given period
                if (customer == "" && ApplicationType == "" && startdate1 != "" && enddate1 != "" &&
                    FacilityType != "" && facilitylimit == "" && tenor == "" && IndustryType == ""
                    && StatusType != "" && interestrate == "" && processingfee == "")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnfacilitytypesByDate() });
                }

                //Returns the facility limits in a given period
                if (customer == "" && ApplicationType == "" && startdate1 != "" && enddate1 != "" &&
                    FacilityType == "" && facilitylimit != "" && tenor == "" && IndustryType == ""
                    && StatusType != "" && interestrate == "" && processingfee == "")
                {
                    return Json (new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnfacilitylimitsByDate() });
                }

                //Returns the facility tenors in a given period
                if (customer == "" && ApplicationType == "" && startdate1 != "" && enddate1 != "" &&
                    FacilityType == "" && facilitylimit == "" && tenor != "" && IndustryType == "" &&
                    StatusType != "" && interestrate == "" && processingfee == "")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnfacilitytenorByDate() }); 
                }
                
                //Returns the Application types in a given period
                if(customer == "" && ApplicationType !="" && startdate1 !="" && enddate1 !="" &&
                    FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType == "" &&
                        StatusType !="" && interestrate == "" && processingfee == "")
                {
                    return Json(new {Result = "OK", Message = "Query Executed Successfully.", Feedback = returnApplicationTypesByDate() });
                }

                //Returns the Collaterals in a particular period
                if(customer == "" && ApplicationType == "" && startdate1 !="" && enddate1 !="" &&
                    FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType == "" &&
                            StatusType !="" && interestrate == "" && processingfee == "" && security !="")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnCollateralByDate() });
                }

                //Returns the Interest Rates in a particular period
                if(customer == "" && ApplicationType == "" && startdate1 !="" && enddate1 !="" &&
                    FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType == "" && StatusType !="" &&
                    interestrate !="" && processingfee == "")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnInterestRatesByDate() });
                }

                //Returns the Processing Fees in a particular period
                if (customer == "" && ApplicationType == "" && startdate1 != "" && enddate1 != "" &&
                    FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType == "" &&
                    StatusType != "" && interestrate == "" && processingfee != "")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnProcessingFeeByDate() });
                }

                //Credit customers in a particular industry
                if (customer == "" && ApplicationType == "" && startdate1 == "" && enddate1 =="" &&
                    FacilityType == "" && facilitylimit == "" && tenor == "" && IndustryType != "" && StatusType == "" && interestrate == ""
                        && processingfee == "")
                {
                    return Json(new { Result = "OK", Message = "Query Executed Successfully.", Feedback = returnCreditCustomersByIndustryByDate() });
                }

                return null;
                }

            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
           
        }
        private string returnCreditCustomersByIndustryByDate()
        {
            try
            {
                if (Session["INDUSTRYTYPE"] != null)
                {
                    string industrytype = Session["INDUSTRYTYPE"].ToString();
                    //DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    //DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());


                    var indtype = from ft in db.IndustryType where ft.industrydescription == industrytype select ft;

                    if (indtype != null)
                    {
                        foreach (var apptyppe in indtype)
                        {
                            appppid = apptyppe.id;
                        }
                        //Select from the creditrequestsummary table for all facility types approved or declined in a given period.

                        var applicantsbrief = from ab in db.ApplicantsBrief
                                              join cd in db.ApplicantType on ab.ApplicantType equals cd.id
                                              join ef in db.IndustryType on ab.Industry equals ef.id
                                              where ab.ApplicantType == cd.id
                                              && ab.Industry == appppid
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


                        app = new List<appbrief>();
                        foreach (var s in applicantsbrief)
                        {
                            app.Add(new appbrief
                            {
                                apptype = s.descritpion,
                                legalidentityname = s.LegalIdentityName,
                                yearofincorporation = s.YearOfIncorporation,
                                rcnumber = s.RCNumber,
                                businessaddress = s.BusinessAddress,
                                nameofauditors = s.NameOfAuditors,
                                industry = s.industrydescription,
                                productrange = s.ProductRange,
                                majorclients = s.MajorClients
                            });
                        }


                    }


                }
                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant Type</th><th>Company Name</th><th>Year of Incorporation</th><th>RC Number</th><th>Business Address</th><th>Name Of Auditors</th><th>Product Range</th><th>Major Clients</th><th>Industry</th></tr>";
                foreach (var f in app)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.apptype + "</td>" + "<td>" + f.legalidentityname + "</td>" + "<td>" + f.yearofincorporation + "</td>" + "<td>" + f.rcnumber + "</td>" + "<td>" + f.businessaddress + "</td>" + "<td>" + f.nameofauditors + "</td>" + "<td>" + f.productrange + "</td>" + "<td>" + f.majorclients + "</td>" + "<td>" + f.industry + "</td></tr>";
                }
                tableStructure = tableStructure + "</table>";
                //return cre;
           
                return tableStructure;
            }

            catch (Exception ree)
            {
                Logging.WriteLog(ree.Message + ":" + ree.StackTrace);
              
            }
            return null;
        }
       

        private string returnProcessingFeeByDate()
        {
            try
            {
                if (Session["PROCESSINGFEE"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string processingfee = Session["PROCESSINGFEE"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var processingfees = from tf in db.CreditRequestSummary
                                         join uf in db.CAM on tf.CAMId equals uf.CAMId
                                         join vf in db.FacilityType on tf.Facility equals vf.id
                                         join wf in db.ApplicationType on tf.ApplicationType equals wf.id
                                         join xf in db.ApplicantsBrief on tf.ApplicantID equals xf.ApplicantID

                                         where tf.ProcessingFee.Contains(processingfee)
                                         && tf.ApplicantID == uf.ApplicantID
                                         && tf.CAMId == uf.CAMId
                                         && tf.ApplicantID == xf.ApplicantID
                                         && tf.CreationDate >= date1 && tf.CreationDate <= date2
                                         && uf.Status == status

                                         select new
                                         {
                                             tf.ApplicantID,
                                             uf.Status,
                                             uf.CAMId,
                                             vf.description,
                                             tf.FacilityAmount,
                                             tf.Tenor,
                                             wf.applicationdescription,
                                             tf.Security,
                                             tf.InterestRate,
                                             tf.ProcessingFee,
                                             xf.LegalIdentityName,
                                             xf.CreationDate
                                         };
                    cre = new List<creditrequestobjects>();
                    foreach (var c in processingfees)
                    {
                        creditrequestobjects cr = new creditrequestobjects();
                        //  cre.Add(new creditrequestobjects
                        // {
                        cr.applicantid = long.Parse(c.ApplicantID.ToString());
                        cr.camid = c.CAMId.ToString();
                        cr.status = c.Status;
                        cr.facilitytype = c.description;
                        cr.facilitylimit =c.FacilityAmount;
                        cr.facilitytenor = c.Tenor;
                        cr.applicationtype = c.applicationdescription;
                        cr.collateral = c.Security;
                        cr.interestrate = c.InterestRate;
                        cr.proposedfees = c.ProcessingFee;
                        cr.customer = c.LegalIdentityName;
                        cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                        cre.Add(cr);
                        // });
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }

                tableStructure = tableStructure + "</table>";
                //return cre;

                return tableStructure;
            }
            catch (Exception p)
            {
                Logging.WriteLog(p.Message + ":" + p.StackTrace);
               
            }
            return null;
        }

        private string returnInterestRatesByDate()
        {
            try
            {
                if (Session["INTERESTRATE"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string interestrate = Session["INTERESTRATE"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var interestrates = from tf in db.CreditRequestSummary
                                        join uf in db.CAM on tf.CAMId equals uf.CAMId
                                        join vf in db.FacilityType on tf.Facility equals vf.id
                                        join wf in db.ApplicationType on tf.ApplicationType equals wf.id
                                        join xf in db.ApplicantsBrief on tf.ApplicantID equals xf.ApplicantID

                                        where tf.InterestRate.Contains(interestrate)
                                        && tf.ApplicantID == uf.ApplicantID
                                        && tf.CAMId == uf.CAMId
                                        && tf.ApplicantID == xf.ApplicantID
                                        && tf.CreationDate >= date1 && tf.CreationDate <= date2
                                        && uf.Status == status

                                        select new
                                             {
                                                 tf.ApplicantID,
                                                 uf.Status,
                                                 uf.CAMId,
                                                 vf.description,
                                                 tf.FacilityAmount,
                                                 tf.Tenor,
                                                 wf.applicationdescription,
                                                 tf.Security,
                                                 tf.InterestRate,
                                                 tf.ProcessingFee,
                                                 xf.LegalIdentityName,
                                                 xf.CreationDate
                                             };
                    cre = new List<creditrequestobjects>();
                    foreach (var c in interestrates)
                    {
                        creditrequestobjects cr = new creditrequestobjects();
                        //  cre.Add(new creditrequestobjects
                        // {
                        cr.applicantid = long.Parse(c.ApplicantID.ToString());
                        cr.camid = c.CAMId.ToString();
                        cr.status = c.Status;
                        cr.facilitytype = c.description;
                        cr.facilitylimit = c.FacilityAmount;
                        cr.facilitytenor = c.Tenor;
                        cr.applicationtype = c.applicationdescription;
                        cr.collateral = c.Security;
                        cr.interestrate = c.InterestRate;
                        cr.proposedfees = c.ProcessingFee;
                        cr.customer = c.LegalIdentityName;
                        cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                        cre.Add(cr);
                        // });
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
                tableStructure = tableStructure + "</table>";

                //return cre;

                return tableStructure;
            }
            catch (Exception t)
            {
                Logging.WriteLog(t.Message + ":" + t.StackTrace);
               
            }
            return null;
            
        }

        private string returnCollateralByDate()
        {
            try
            {
                if (Session["SECURITY"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string collateral = Session["SECURITY"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var collateralsecurity = from tf in db.CreditRequestSummary
                                             join uf in db.CAM on tf.CAMId equals uf.CAMId
                                             join vf in db.FacilityType on tf.Facility equals vf.id
                                             join wf in db.ApplicationType on tf.ApplicationType equals wf.id
                                             join xf in db.ApplicantsBrief on tf.ApplicantID equals xf.ApplicantID

                                             where tf.Security.Contains(collateral)
                                             && tf.ApplicantID == uf.ApplicantID
                                             && tf.CAMId == uf.CAMId
                                             && tf.ApplicantID == xf.ApplicantID
                                             && tf.CreationDate >= date1 && tf.CreationDate <= date2
                                             && uf.Status == status

                                             select new
                                                  {
                                                      tf.ApplicantID,
                                                      uf.Status,
                                                      uf.CAMId,
                                                      vf.description,
                                                      tf.FacilityAmount,
                                                      tf.Tenor,
                                                      wf.applicationdescription,
                                                      tf.Security,
                                                      tf.InterestRate,
                                                      tf.ProcessingFee,
                                                      xf.LegalIdentityName,
                                                      xf.CreationDate
                                                  };
                    cre = new List<creditrequestobjects>();
                    foreach (var c in collateralsecurity)
                    {
                        creditrequestobjects cr = new creditrequestobjects();
                        //  cre.Add(new creditrequestobjects
                        // {
                        cr.applicantid = long.Parse(c.ApplicantID.ToString());
                        cr.camid = c.CAMId.ToString();
                        cr.status = c.Status;
                        cr.facilitytype = c.description;
                        cr.facilitylimit = c.FacilityAmount;
                        cr.facilitytenor = c.Tenor;
                        cr.applicationtype = c.applicationdescription;
                        cr.collateral = c.Security;
                        cr.interestrate = c.InterestRate;
                        cr.proposedfees = c.ProcessingFee;
                        cr.customer = c.LegalIdentityName;
                        cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                        cre.Add(cr);
                        // });
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
               // tableStructure = tableStructure + "</table>" + "<br />" + "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                tableStructure = tableStructure + "</table>";

                //return cre;

                return tableStructure;
            }
            catch (Exception c)
            {
                Logging.WriteLog(c.Message + ":" + c.StackTrace);
                
            }
            return null; 
        }

        private string returnApplicationTypesByDate()
        {
            try
            {
                if (Session["APPLICATIONTYPE"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string applicanttype = Session["APPLICATIONTYPE"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var apptype = from ft in db.ApplicationType where ft.applicationdescription == applicanttype select ft;

                    if (apptype != null)
                    {
                        foreach (var apptyppe in apptype)
                        {
                            appppid = apptyppe.id;
                        }
                        //Select from the creditrequestsummary table for all facility types approved or declined in a given period.
                        var facilityyytyppe = from ftt in db.CreditRequestSummary
                                              join gtt in db.CAM on ftt.CAMId equals gtt.CAMId
                                              join htt in db.ApplicationType on ftt.ApplicationType equals htt.id
                                              join itt in db.FacilityType on ftt.Facility equals itt.id
                                              join jtt in db.ApplicantsBrief on ftt.ApplicantID equals jtt.ApplicantID

                                              where ftt.ApplicationType == appppid
                                              && ftt.ApplicantID == gtt.ApplicantID
                                              && ftt.CAMId == gtt.CAMId
                                              && ftt.ApplicantID == jtt.ApplicantID
                                              && ftt.Facility == itt.id
                                              && gtt.CreationDate >= date1 && gtt.CreationDate <= date2
                                              && gtt.Status == status


                                              select new
                                              {

                                                  ftt.ApplicantID,
                                                  gtt.Status,
                                                  gtt.CAMId,
                                                  htt.applicationdescription,
                                                  ftt.FacilityAmount,
                                                  ftt.Tenor,
                                                  itt.description,
                                                  ftt.Security,
                                                  ftt.InterestRate,
                                                  ftt.ProcessingFee,
                                                  jtt.LegalIdentityName,
                                                  gtt.CreationDate
                                              };
                        cre = new List<creditrequestobjects>();
                        foreach (var c in facilityyytyppe)
                        {
                            creditrequestobjects cr = new creditrequestobjects();
                            //  cre.Add(new creditrequestobjects
                            // {
                            cr.applicantid = long.Parse(c.ApplicantID.ToString());
                            cr.camid = c.CAMId.ToString();
                            cr.status = c.Status;
                            cr.facilitytype = c.description;
                            cr.facilitylimit = c.FacilityAmount;
                            cr.facilitytenor = c.Tenor;
                            cr.applicationtype = c.applicationdescription;
                            cr.collateral = c.Security;
                            cr.interestrate = c.InterestRate;
                            cr.proposedfees = c.ProcessingFee;
                            cr.customer = c.LegalIdentityName;
                            cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                            cre.Add(cr);
                            // });
                        }
                    }
                }
                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
              //  tableStructure = tableStructure + "</table>" + "<br />" + "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                tableStructure = tableStructure + "</table>";

                
                //return cre;

                return tableStructure;
            }
            catch (Exception z)
            {
                Logging.WriteLog(z.Message + ":" + z.StackTrace);
               
            }
            return null;
            
        }


        private string returnfacilitytenorByDate()
        {
            try
            {
                if (Session["TENOR"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string tenor = Session["TENOR"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var tenorfacility = from tf in db.CreditRequestSummary
                                        join uf in db.CAM on tf.CAMId equals uf.CAMId
                                        join vf in db.FacilityType on tf.Facility equals vf.id
                                        join wf in db.ApplicationType on tf.ApplicationType equals wf.id
                                        join xf in db.ApplicantsBrief on tf.ApplicantID equals xf.ApplicantID

                                        where tf.Tenor.Contains(tenor)
                                        && tf.ApplicantID == uf.ApplicantID
                                        && tf.CAMId == uf.CAMId
                                        && tf.ApplicantID == xf.ApplicantID
                                        && tf.CreationDate >= date1 && tf.CreationDate <= date2
                                        && uf.Status == status

                                        select new
                                             {
                                                 tf.ApplicantID,
                                                 uf.Status,
                                                 uf.CAMId,
                                                 vf.description,
                                                 tf.FacilityAmount,
                                                 tf.Tenor,
                                                 wf.applicationdescription,
                                                 tf.Security,
                                                 tf.InterestRate,
                                                 tf.ProcessingFee,
                                                 xf.LegalIdentityName,
                                                 xf.CreationDate
                                             };
                    cre = new List<creditrequestobjects>();
                    foreach (var c in tenorfacility)
                    {
                        creditrequestobjects cr = new creditrequestobjects();
                        //  cre.Add(new creditrequestobjects
                        // {
                        cr.applicantid = long.Parse(c.ApplicantID.ToString());
                        cr.camid = c.CAMId.ToString();
                        cr.status = c.Status;
                        cr.facilitytype = c.description;
                        cr.facilitylimit = c.FacilityAmount;
                        cr.facilitytenor = c.Tenor;
                        cr.applicationtype = c.applicationdescription;
                        cr.collateral = c.Security;
                        cr.interestrate = c.InterestRate;
                        cr.proposedfees = c.ProcessingFee;
                        cr.customer = c.LegalIdentityName;
                        cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                        cre.Add(cr);
                        // });
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
               // tableStructure = tableStructure + "</table>" + "<br />" + "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                tableStructure = tableStructure + "</table>";

                //return cre;

                return tableStructure;

            }
            catch (Exception tt)
            {
                Logging.WriteLog(tt.Message + ":" + tt.StackTrace);
              
            }
            return null;

            
        }
    
        private string returnfacilitylimitsByDate()
        {
            try
            {
                if (Session["FACILITYLIMIT"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    decimal facilitylimit = Convert.ToDecimal(Session["FACILITYLIMIT"].ToString());
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var facilitylimits = from fl in db.CreditRequestSummary
                                         join gl in db.CAM on fl.CAMId equals gl.CAMId
                                         join hl in db.FacilityType on fl.Facility equals hl.id
                                         join il in db.ApplicationType on fl.ApplicationType equals il.id
                                         join jl in db.ApplicantsBrief on fl.ApplicantID equals jl.ApplicantID

                                         where fl.FacilityAmount == facilitylimit
                                         && fl.ApplicantID == gl.ApplicantID
                                         && fl.CAMId == gl.CAMId
                                         && fl.ApplicantID == jl.ApplicantID
                                         && fl.CreationDate >= date1 && fl.CreationDate <= date2
                                         && gl.Status == status

                                         select new
                                             {
                                                 fl.ApplicantID,
                                                 gl.Status,
                                                 gl.CAMId,
                                                 hl.description,
                                                 fl.FacilityAmount,
                                                 fl.Tenor,
                                                 il.applicationdescription,
                                                 fl.Security,
                                                 fl.InterestRate,
                                                 fl.ProcessingFee,
                                                 jl.LegalIdentityName,
                                                 gl.CreationDate
                                             };
                    cre = new List<creditrequestobjects>();
                    foreach (var c in facilitylimits)
                    {
                        creditrequestobjects cr = new creditrequestobjects();
                        //  cre.Add(new creditrequestobjects
                        // {
                        cr.applicantid = long.Parse(c.ApplicantID.ToString());
                        cr.camid = c.CAMId.ToString();
                        cr.status = c.Status;
                        cr.facilitytype = c.description;
                        cr.facilitylimit = c.FacilityAmount;
                        cr.facilitytenor = c.Tenor;
                        cr.applicationtype = c.applicationdescription;
                        cr.collateral = c.Security;
                        cr.interestrate = c.InterestRate;
                        cr.proposedfees = c.ProcessingFee;
                        cr.customer = c.LegalIdentityName;
                        cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                        cre.Add(cr);
                        // });
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
               // tableStructure = tableStructure + "</table>" + "<br />" + "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                tableStructure = tableStructure + "</table>";

                
                //return cre;

                return tableStructure;

            }
            catch (Exception zst)
            {
                Logging.WriteLog(zst.Message + ":" + zst.StackTrace);
                
            }
            return null;
            }
        

        private string returnfacilitytypesByDate()
        {
            try
            {
                if (Session["FACILITYTYPE"] != null && Session["STARTDATE1"] != null && Session["ENDDATE1"] != null
                    && Session["STATUS"] != null)
                {
                    string facilitytype = Session["facilitytype"].ToString();
                    DateTime date1 = DateTime.Parse(Session["STARTDATE1"].ToString());
                    DateTime date2 = DateTime.Parse(Session["ENDDATE1"].ToString());
                    string status = Session["STATUS"].ToString();

                    var facilityytype = from ft in db.FacilityType where ft.description == facilitytype select ft;

                    if (facilityytype != null)
                    {
                        foreach (var facilityyytype in facilityytype)
                        {
                            facid = facilityyytype.id;
                        }
                        //Select from the creditrequestsummary table for all facility types approved or declined in a given period.
                        var facilityyytyppe = from ftt in db.CreditRequestSummary
                                              join gtt in db.CAM on ftt.CAMId equals gtt.CAMId
                                              join htt in db.FacilityType on ftt.Facility equals htt.id
                                              join itt in db.ApplicationType on ftt.ApplicationType equals itt.id
                                              join jtt in db.ApplicantsBrief on ftt.ApplicantID equals jtt.ApplicantID

                                              where ftt.Facility == facid
                                              && ftt.ApplicantID == gtt.ApplicantID
                                              && ftt.CAMId == gtt.CAMId
                                              && ftt.ApplicantID == jtt.ApplicantID
                                              && gtt.CreationDate >= date1 && gtt.CreationDate <= date2
                                              && gtt.Status == status


                                              select new
                                              {
                                                  ftt.ApplicantID,
                                                  gtt.Status,
                                                  gtt.CAMId,
                                                  htt.description,
                                                  ftt.FacilityAmount,
                                                  ftt.Tenor,
                                                  itt.applicationdescription,
                                                  ftt.Security,
                                                  ftt.InterestRate,
                                                  ftt.ProcessingFee,
                                                  jtt.LegalIdentityName,
                                                  gtt.CreationDate
                                              };
                        cre = new List<creditrequestobjects>();
                        foreach (var c in facilityyytyppe)
                        {
                            creditrequestobjects cr = new creditrequestobjects();
                            //  cre.Add(new creditrequestobjects
                            // {
                            cr.applicantid = long.Parse(c.ApplicantID.ToString());
                            cr.camid = c.CAMId.ToString();
                            cr.status = c.Status;
                            cr.facilitytype = c.description;
                            cr.facilitylimit = c.FacilityAmount;
                            cr.facilitytenor = c.Tenor;
                            cr.applicationtype = c.applicationdescription;
                            cr.collateral = c.Security;
                            cr.interestrate = c.InterestRate;
                            cr.proposedfees = c.ProcessingFee;
                            cr.customer = c.LegalIdentityName;
                            cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                            cre.Add(cr);
                            // });
                        }
                    }
                }
                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
             //   tableStructure = tableStructure + "</table>" + "<br />" + "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";

                tableStructure = tableStructure + "</table>";

                //return cre;

                return tableStructure;
            }
            catch (Exception psp)
            {
                Logging.WriteLog(psp.Message + ":" + psp.StackTrace);
               
            }
            return null;
        }         
        
        private string returnCreditRequestsByDate()
        {
            try
            {
                if (Session["CUSTOMER"] != null)
                {
                    string customer = Session["CUSTOMER"].ToString();

                    var creditrequests = from ab in db.ApplicantsBrief where ab.LegalIdentityName.Contains(customer) select ab;
                    if (creditrequests != null)
                    {
                        foreach (var creditrequestss in creditrequests)
                        {
                            appid = creditrequestss.ApplicantID;
                            //return the the number of applicant types in the appid list object

                            var applicanttypes = from at in db.CreditRequestSummary
                                                 join bt in db.CAM on at.CAMId equals bt.CAMId
                                                 join ct in db.FacilityType on at.Facility equals ct.id
                                                 join dt in db.ApplicationType on at.ApplicationType equals dt.id
                                                 join et in db.ApplicantsBrief on at.ApplicantID equals et.ApplicantID

                                                 where at.ApplicantID == appid
                                                 && at.Facility == ct.id
                                                 && at.CAMId == bt.CAMId
                                                 && at.ApplicantID == et.ApplicantID

                                                 select new
                                                 {
                                                     at.ApplicantID,
                                                     bt.Status,
                                                     bt.CAMId,
                                                     ct.description,
                                                     at.FacilityAmount,
                                                     at.Tenor,
                                                     dt.applicationdescription,
                                                     at.Security,
                                                     at.InterestRate,
                                                     at.ProcessingFee,
                                                     et.LegalIdentityName,
                                                     bt.CreationDate
                                                 };

                            cre = new List<creditrequestobjects>();
                            foreach (var c in applicanttypes)
                            {
                                creditrequestobjects cr = new creditrequestobjects();
                                //  cre.Add(new creditrequestobjects
                                // {
                                cr.applicantid = long.Parse(c.ApplicantID.ToString());
                                cr.camid = c.CAMId.ToString();
                                cr.status = c.Status;
                                cr.facilitytype = c.description;
                                cr.facilitylimit = c.FacilityAmount;
                                cr.facilitytenor = c.Tenor;
                                cr.applicationtype = c.applicationdescription;
                                cr.collateral = c.Security;
                                cr.interestrate = c.InterestRate;
                                cr.proposedfees = c.ProcessingFee;
                                cr.customer = c.LegalIdentityName;
                                cr.DateCreated = DateTime.Parse(c.CreationDate.ToString());

                                cre.Add(cr);
                                // });
                            }

                        }
                    }
                }

                string tableStructure = "<table class=\"tblboda1\"><tr><th>Applicant ID</th><th>Cam Id</th><th>Status</th><th>Facility Type</th><th>Facility Limit</th><th>Facility Tenor</th><th>Application Type</th><th>Collateral</th><th>Interest Rate</th><th>Proposed Fees</th><th>Customer</th><th>Date Created</th></tr>";
                foreach (var f in cre)
                {
                    tableStructure = tableStructure + "<tr><td>" + f.applicantid + "</td>" + "<td>" + f.camid + "</td>" + "<td>" + f.status + "</td>" + "<td>" + f.facilitytype + "</td>" + "<td>" + f.facilitylimit + "</td>" + "<td>" + f.facilitytenor + "</td>" + "<td>" + f.applicationtype + "</td>" + "<td>" + f.collateral + "</td>" + "<td>" + f.interestrate + "</td>" + "<td>" + f.proposedfees + "</td>" + "<td>" + f.customer + "</td>" + "<td>" + f.DateCreated + "</td></tr>";
                }
              //  tableStructure = tableStructure + "</table>" + "<br />"+ "<br />" + "<input type=\"button\" id=\"btnExport1\" value=\"Export To Excel\" />";
                //return cre;
                tableStructure = tableStructure + "</table>";

                
                return tableStructure;
            }
            catch (Exception ttt)
            {
                Logging.WriteLog(ttt.Message + ":" + ttt.StackTrace);
               
            }
            return null;
        }
        //Post
        [HttpPost]
        public ActionResult Index(string Regions, string Status, string startdate, string enddate)
        {
            if (Regions != "")
            {
                Session["regionselect"] = Regions;
            }

            if (Status != "")
            {
                Session["statusselect"] = Status;
            }
            if (startdate != "")
            {
                Session["startdate"] = startdate;
            }
            if (enddate != "")
            {
                Session["enddate"] = enddate;
            }
            try
            {
                regiontypes = new List<string>();
                statustypes = new List<string>();
                FacilityTypes = new List<string>();
                ApplicationType = new List<string>();
                IndustryType = new List<string>();

                var reg = from rr in db.Region select rr;
                if (reg != null)
                {
                    var regions = reg.ToList();
                    foreach (var regionss in regions)
                    {
                        string regionname = regionss.RegionName;
                        regiontypes.Add(regionname);
                    }
                }

                //Status dropdwon
                var status = from ss in db.State select ss;
                if (status != null)
                {
                    var stattus = status.ToList();
                    foreach (var stattuss in stattus)
                    {
                        string statusname = stattuss.description;
                        statustypes.Add(statusname);
                    }
                }

                //Facility Types dropdown
                var facilitytypes = from ft in db.FacilityType select ft;
                if (facilitytypes != null)
                {
                    var facillitytypes = facilitytypes.ToList();
                    foreach (var facillitytypess in facillitytypes)
                    {
                        string factypes = facillitytypess.description;
                        FacilityTypes.Add(factypes);
                    }

                }

                //Application Types dropdown
                var applicationtypes = from at in db.ApplicationType select at;
                if (applicationtypes != null)
                {
                    var applicationntypes = applicationtypes.ToList();
                    foreach (var appllicationtypes in applicationntypes)
                    {
                        string apptypes = appllicationtypes.applicationdescription;
                        ApplicationType.Add(apptypes);
                    }

                }

                //Industry Types dropdown
                var industrytypes = from it in db.IndustryType select it;
                if (industrytypes != null)
                {
                    var industryytypes = industrytypes.ToList();
                    foreach (var industryytyype in industryytypes)
                    {
                        string indtypes = industryytyype.industrydescription;
                        IndustryType.Add(indtypes);
                    }

                }

                ViewBag.Regions = new SelectList(regiontypes);
                ViewBag.Status = new SelectList(statustypes);
                ViewBag.FacilityType = new SelectList(FacilityTypes);
                ViewBag.ApplicationType = new SelectList(ApplicationType);
                ViewBag.IndustryType = new SelectList(IndustryType);

                //
                //if (Regions == "")
                //{
                   
                //}
                //else if (Status == "")
                //{

                //}
                if (Regions != "" && Status == "" && startdate != "" && enddate != "")
                {
                    //ViewBag.RegionsCredit = returnCreditsByRegion();
                    ViewBag.RegionsCredit = returnCreditsByRegionAndDate();
                }
                else if (Status !="" && Regions == "" && startdate != "" && enddate != "")
                {
                    //ViewBag.RegionsCredit = returnCreditsByRegion();
                    ViewBag.RegionsCredit = returnCreditsByStatusAndDate();
                }
                else if (Regions != "" && Status == "" && startdate == "" && enddate == "")
                {
                    ViewBag.RegionsCredit = returnCreditsByRegion();
                }
                else if (Regions != "" && Status != "" && startdate == "" && enddate == "")
                {
                    ViewBag.RegionsCredit = returnCreditsForRegionByStatus();
                }
                else if (Regions == "" && Status != "" && startdate == "" && enddate == "")
                {
                    ViewBag.RegionsCredit = returnbyStatus();
                }
                else if (Regions != "" && Status != "" && startdate != "" && enddate != "")
                {
                    ViewBag.RegionsCredit = returnCreditsForAllFields();
                }
                else if (Regions == "" && Status == "" && startdate == "" && enddate == "")
                {
                    ViewBag.RegionsCredit = returnAllCAMs();
                }
                //else if (Regions != "" && Status == "" && startdate != "" && enddate !="")
                //{
                //    ViewBag.RegionsCredit = returnCreditsByRegionAndDate();
                //}
                //else if(Regions !="" && Status != "")
                //{
                //    ViewBag.RegionsCreditStatus = returnCreditsForRegionByStatus();
                //}
            }
            catch (Exception ep)
            {
                Logging.WriteLog(ep.Message + ":" + ep.StackTrace);
                //Redirect user to error page.
                return RedirectToAction("Exception", "CAMForm");
            }
           // return View(db.CAM.ToList());      
            return View();
        }

        //Return all Credits done by region
        public List<CAM> returnCreditsByRegion()
        {
            try
            {
                if (Session["regionselect"] != null)
                {
                    string regionselected = Session["regionselect"].ToString();


                    var creditrequestsregion = from crr in db.CAM where crr.RegionProcessedFrom == regionselected select crr;

                    if (creditrequestsregion != null)
                    {
                        return creditrequestsregion.ToList();

                    }
                }

                else if (Session["statusselect"] != null && Session["regionselect"] != null)
                {
                    string statusselected = Session["statusselect"].ToString();
                    string regionselected = Session["regionselect"].ToString();

                    var creditrequestsregion = from crr in db.CAM where crr.RegionProcessedFrom == regionselected && crr.Status == statusselected select crr;
                    if (creditrequestsregion != null)
                    {
                        return creditrequestsregion.ToList();
                    }

                }
            }
            catch (Exception c)
            {
                Logging.WriteLog(c.Message + ":" + c.StackTrace);
              
            }
                return null;
            
        }

        //Return all Credits done in a region of a particular status.
        public List<CAM> returnCreditsForRegionByStatus()
        {
            try
            {
                string regionselected = Session["regionselect"].ToString();
                string statusselected = Session["statusselect"].ToString();


                var creditrequestsregionbystatus = from crr in db.CAM where crr.RegionProcessedFrom == regionselected && crr.Status == statusselected select crr;
                if (creditrequestsregionbystatus != null)
                {
                    return creditrequestsregionbystatus.ToList();
                }
            }
            catch (Exception eeee)
            {

                Logging.WriteLog(eeee.Message + ":" + eeee.StackTrace);
                
            }
            return null;
        }

        //Return all credits of a particular status bankwide.
        public List<CAM> returnbyStatus()
        {
            try
            {
                if (Session["statusselect"] != null)
                {
                    string statusselected = Session["statusselect"].ToString();


                    var creditrequestsregion = from crr in db.CAM where crr.Status == statusselected select crr;
                    if (creditrequestsregion != null)
                    {
                        return creditrequestsregion.ToList();
                    }
                }
            }
            catch (Exception tre)
            {
                Logging.WriteLog(tre.Message + ":" + tre.StackTrace);
                
            }
            return null;
        }

        //Return all credits from a particular region by date
        public List<CAM> returnCreditsByRegionAndDate()
        {
            try
            {

                if (Session["regionselect"] != null)
                {
                    string regionselected = Session["regionselect"].ToString();
                    DateTime incomingstartdate = DateTime.Parse(Session["startdate"].ToString());
                    DateTime incomingenddate = DateTime.Parse(Session["enddate"].ToString());


                    var creditrequestsregion = from crr in db.CAM where crr.RegionProcessedFrom == regionselected && crr.CreationDate >= incomingstartdate && crr.CreationDate <= incomingenddate select crr;
                    // creditrequestsregion.OrderBy(x => x.CreationDate);
                    if (creditrequestsregion != null)
                    {
                        return creditrequestsregion.ToList();
                    }
                }
            }
            catch (Exception s)
            {
                Logging.WriteLog(s.Message + ":" + s.StackTrace);
               
            }
            return null;
        }

        //Return all credits of a certain status by date
        public List<CAM> returnCreditsByStatusAndDate()
        {
            if (Session["statusselect"] != null)
            {
                string statusselect = Session["statusselect"].ToString();
                DateTime incomingstartdate = DateTime.Parse(Session["startdate"].ToString());
                DateTime incomingenddate = DateTime.Parse(Session["enddate"].ToString());


                var creditrequestsregion = from crr in db.CAM where crr.Status == statusselect && crr.CreationDate >= incomingstartdate && crr.CreationDate <= incomingenddate select crr;
                // creditrequestsregion.OrderBy(x => x.CreationDate);
                if (creditrequestsregion != null)
                {
                    return creditrequestsregion.ToList();
                }
            }
            return null;
        }

       //Return credits for all fields
        public List<CAM> returnCreditsForAllFields()
        {
            try
            {
                if (Session["regionselect"] != null && Session["statusselect"] != null && Session["startdate"] != null && Session["enddate"] != null)
                {
                    string regionselected = Session["regionselect"].ToString();
                    string statusselect = Session["statusselect"].ToString();
                    DateTime incomingstartdate = DateTime.Parse(Session["startdate"].ToString());
                    DateTime incomingenddate = DateTime.Parse(Session["enddate"].ToString());

                    var creditrequestsforall = from crr in db.CAM where crr.RegionProcessedFrom == regionselected && crr.Status == statusselect && crr.CreationDate >= incomingstartdate && crr.CreationDate <= incomingenddate select crr;

                    if (creditrequestsforall != null)
                    {
                        return creditrequestsforall.ToList();
                    }

                }
            }
            catch (Exception a)
            {
                Logging.WriteLog(a.Message + ":" + a.StackTrace);
               
            }
            return null;
        }

        //If no filter is selected, return all CAMs bankwide.
        public List<CAM> returnAllCAMs()
        {
            try
            {
                // if (Session["regionselect"] == null && Session["statusselect"] == null && Session["startdate"] == null && Session["enddate"] == null)
                //  {
                var returnallcams = from crr in db.CAM select crr;
                if (returnallcams != null)
                {
                    return returnallcams.ToList();
                }
                //  }
            }
           catch(Exception o)
            {
                Logging.WriteLog(o.Message + ":" + o.StackTrace);
              
            }
            return null;
        }
        //
        // GET: /CAMReports/Details/5

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
        // GET: /CAMReports/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CAMReports/Create

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
        // GET: /CAMReports/Edit/5

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
        // POST: /CAMReports/Edit/5

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
        // GET: /CAMReports/Delete/5

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
        // POST: /CAMReports/Delete/5

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