using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Mail;
using System.Web;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
//using System.Web.UI.UserControl;
//using Microsoft.ApplicationBlocks.Data;
using System.Data.Common;
//using System.Web.SessionState.HttpSessionState;
using System.Net;
using System.Threading;
using System.Runtime.Remoting;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

public class Logging
{
    #region "ERROR LOG"
    HttpContext Context = HttpContext.Current;


    public static void WriteLog(string msg)
    {
        HttpContext context = HttpContext.Current;
        string _path = null;
        _path = "~/ErrorLog\\ErrorLog.txt";
        string path = context.Server.MapPath(_path);
        System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true);
        writer.WriteLine(msg);
        writer.WriteLine(DateTime.Now);
        writer.Close();
    }
    #endregion
}