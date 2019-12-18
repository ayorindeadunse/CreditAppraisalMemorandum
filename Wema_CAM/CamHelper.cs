using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Wema_CAM.Models;

namespace Wema_CAM
{
    public class CamHelper
    {

        private static Wema_CAMEntities db = new Wema_CAMEntities();

        //Corporate Credits
        public static List<CAM_User> GetNextCorporateUserList(string userGrade, string branchId)
        {
            string sqlCmd = "select ";
            string nextLevelUserGroup = "";


            switch (userGrade)
            {
                case "RM":
                    nextLevelUserGroup = "UH";
                    sqlCmd = "Select * from CAM_USer where grade = '" + nextLevelUserGroup + "' and branchid='" + branchId + "'";
                    break;
                case "UH":
                    nextLevelUserGroup = "GH";
                    sqlCmd = "Select * from CAM_USer where grade = '" + nextLevelUserGroup+"'";
                    break;
                case "GH":
                    nextLevelUserGroup = "CRA";
                    sqlCmd = "Select * from CAM_USer where grade = '" + nextLevelUserGroup + "' and branchid='" + branchId + "'";
                    break;
                //Case ZM
                case "CRA":
                    //nextLevelUserGroup = "RE";
                    sqlCmd = "Select * from CAM_User";
                    break;
                case "RE":
                    nextLevelUserGroup = "CRO";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "' and regionid=(select regionid from branch where branchid ='" + branchId + "')";
                    break;
                case "CRO":
                    nextLevelUserGroup = "ED";
                    // sqlCmd = "Select * from CAM_User where grade = '"+nextLevelUserGroup+"' and regionid = (select regionid from branch where branchid ='"+branchId+"')";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "'";
                    break;
                case "ED":
                    // nextLevelUserGroup = "MD/CEO";
                    sqlCmd = "Select * from CAM_User where grade in ('MD/CEO','ED')";
                    break;
                case "MD/CEO":
                    sqlCmd = "Select * from CAM_User where grade = 'MD/CEO'";
                    break;

                default:
                    break;

            }
            return db.CAM_User.SqlQuery(sqlCmd).ToList();
        }

        //Commercial Credits
        public static List<CAM_User> GetNextLevelUserList(string userGrade, string branchId)
        {
            string sqlCmd = "select ";
            string nextLevelUserGroup = "";

           
            switch(userGrade)
            {
                case "RM":
                    nextLevelUserGroup="BDM";
                   sqlCmd = "Select * from CAM_USer where grade = '"+nextLevelUserGroup+"' and branchid='"+branchId+"'";
                    break;
                case "BDM":
                    nextLevelUserGroup="ZM";
                     sqlCmd = "Select * from CAM_USer where grade = '"+nextLevelUserGroup+"' and zoneid=(select zoneid from branch where branchid ='"+branchId+"')";
                     break;
                    //Case ZM
                case "CRA":
                    //nextLevelUserGroup = "RE";
                    sqlCmd = "Select * from CAM_User";
                    break;
                case "RE":
                    nextLevelUserGroup = "CRO";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "' and regionid=(select regionid from branch where branchid ='" + branchId + "')";
                    break;
                case "CRO":
                    nextLevelUserGroup = "ED";
                   // sqlCmd = "Select * from CAM_User where grade = '"+nextLevelUserGroup+"' and regionid = (select regionid from branch where branchid ='"+branchId+"')";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "'";
                   break;
                case "ED":
                   // nextLevelUserGroup = "MD/CEO";
                    sqlCmd = "Select * from CAM_User where grade in ('MD/CEO','ED')";
                    break; 
                case "MD/CEO":
                    sqlCmd = "Select * from CAM_User where grade = 'MD/CEO'";
                    break;
      
                default:
                    break;

            }
            return db.CAM_User.SqlQuery(sqlCmd).ToList();
        }

        public static List<CAM_User> GetNextLevelUserList(string userGrade, string branchId, decimal TotalFacilityAmount)
        {
            string sqlCmd = "select ";
            string nextLevelUserGroup = "";

            switch(userGrade)
            {
                case "RM":
                    nextLevelUserGroup = "BDM";
                    sqlCmd = "Select * from CAM_USer where grade = '" + nextLevelUserGroup + "' and branchid='" + branchId + "'";
                    break;
                case "BDM":
                    nextLevelUserGroup = "ZM";
                    sqlCmd = "Select * from CAM_USer where grade = '" + nextLevelUserGroup + "' and zoneid=(select zoneid from branch where branchid ='" + branchId + "')";
                    break;
                //Case ZM
                case "CRA":
                    //nextLevelUserGroup = "RE";
                    sqlCmd = "Select * from CAM_User";
                    break;
                case "RE":
                    nextLevelUserGroup = "CRO";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "' and regionid=(select regionid from branch where branchid ='" + branchId + "')";
                    break;
                case "CRO":
                    nextLevelUserGroup = "ED";
                    // sqlCmd = "Select * from CAM_User where grade = '"+nextLevelUserGroup+"' and regionid = (select regionid from branch where branchid ='"+branchId+"')";
                    sqlCmd = "Select * from CAM_User where grade = '" + nextLevelUserGroup + "'";
                    break;
                case "ED":
                    // nextLevelUserGroup = "MD/CEO";
                    sqlCmd = "Select * from CAM_User where grade in ('MD/CEO','ED')";
                    break;                           
                    case "ZM":
                    if (TotalFacilityAmount > 5000000)
                        sqlCmd = "Select * from CAM_USer where grade = 'CRA'";
                    else if (TotalFacilityAmount < 5000000)
                        sqlCmd = "Select * from CAM_USer where grade = 'RE' and regionid=(select regionid from branch where branchid ='" + branchId + "')";
                    break;
                    case "MD/CEO":
                    sqlCmd = "Select * from CAM_User where grade = 'MD/CEO'";
                    break;
      
            
                default:
                    break;

            }
            return db.CAM_User.SqlQuery(sqlCmd).ToList();
        }

        public void alertNextOnQueue(string username, string NextLevelSupervisorEmail, string preUser)
        {
            try
            {
               
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(NextLevelSupervisorEmail);
                mail.Subject = "Credit Appraisal Memorandum Request";
                mail.Body = "Hi " + username + "," + "\r\n\n" + "You have a pending Credit Appraisal Memorandum(CAM) request on your queue which was initiated by " + preUser + ".Kindly log into the application " + "<a href='http://172.27.4.227/WemaCAM'>Login</a>" + " to view the request and treat." + "\r\n\n" + "Regards," + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.\
           
            }


        }

        public void declineTransactionMail(string NextLevelSupervisorEmail, string preUser)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(NextLevelSupervisorEmail);
                mail.Subject = "Credit Appraisal Memorandum Request";
                mail.Body = "Hi " + preUser + "," + "\r\n\n" + "Your Credit Appraisal Memorandum(CAM) request was declined." + "Kindly log into the application " + "<a href='http://172.27.4.227/WemaCAM'>Login</a>" + " to re-initiate the request or send a new CAM." + "\r\n\n" + "Regards," + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
            }


        }

        public void reviewTransactionMail(string NextLevelSupervisorEmail, string preUser)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(NextLevelSupervisorEmail);
                mail.Subject = "Credit Appraisal Memorandum Request";
                mail.Body = "Hi " + preUser + "," + "\r\n\n" + "Your Credit Appraisal Memorandum(CAM) has been sent back to you for you to make corrections and review." + "Kindly log into the application " + "<a href='http://172.27.4.227/WemaCAM'>Login</a>" + " to make the necessary corrections and re-submit." + "\r\n\n" + "Regards," + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
            }


        }

        public void approvedTransactionMail(string NextLevelSupervisorEmail, string preUser,long camid)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                mail.To.Add(NextLevelSupervisorEmail);
                mail.Subject = "Credit Appraisal Memorandum Request";
                mail.Body = "Hi " + preUser + "," + "\r\n\n" + "Your Credit Appraisal Memorandum(CAM) " + camid + " has been Approved." +  "Regards," + "\r\n\n" + "The Credit Risk Management Team.";
                mail.IsBodyHtml = true;
                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                //SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["emailpass"]);
                // SmtpServer.Credentials = new System.Net.ne
                //SmtpServer.EnableSsl = true;
                //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Logging.WriteLog(e.Message + ":" + e.StackTrace);
                //Redirect user to error page.
            }


        }
    }
}