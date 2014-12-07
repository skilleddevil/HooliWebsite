using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;
using Hooli.Models;
using Hooli.MySql;
using System.Web.Security;

namespace Hooli.Controllers
{
    public class AddNewSoftwareController : Controller
    {
        //
        // GET: /AddNewSoftware/

        [Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            if (Roles.IsUserInRole("Admin"))
            {
                return View();
            }
            return View("UnauthorizedAccess");
        }
        [Authorize(Roles="Admin")]
        public ActionResult Save(FormCollection formCollection, SoftwareModel model)
        {
            if(Request != null)
            {
                HttpPostedFileBase file = model.fileName;
           
                if((file!=null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileType = file.ContentType;
                    if (fileType.Contains("application"))
                    {
                        ViewBag.Error = "No executable files can be uploaded.";
                        return View("UnsuccessfulUpload");
                    }

                    BinaryReader br = new BinaryReader(file.InputStream);
                    Byte[] fileBytes = br.ReadBytes(file.ContentLength);
                    br.Close();                    

                    //Construct query 
                    string query = "insert into Software(admin_id, softwareName, fileName, version, date_added, description, data, contentType) values " + 
                                   "(@adminid, @softwareName, @fileName, @version, @date, @description, @data, @fileContentType);";
                    MySqlCommand cmd = new MySqlCommand(query);
                    cmd.Parameters.Add("@adminid", MySqlDbType.Int16).Value = (int)Membership.GetUser().ProviderUserKey;
                    cmd.Parameters.Add("@softwareName", MySqlDbType.String).Value = model.softwareName;
                    cmd.Parameters.Add("@fileName", MySqlDbType.String).Value = file.FileName;
                    cmd.Parameters.Add("@version", MySqlDbType.String).Value = model.version;
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.Add("@description", MySqlDbType.Text).Value = model.description;
                    cmd.Parameters.Add("@data", MySqlDbType.Blob).Value = fileBytes;
                    cmd.Parameters.Add("@fileContentType", MySqlDbType.String).Value = file.ContentType;

                    //Save data to db
                    DBConnect db = new DBConnect();
                    db.Insert(cmd);

                    return View("Success");
                }
                else
                {
                    ViewBag.Error = "Error in processing the file.";
                    return View("UnsuccessfulUpload");
                }
            }
            else
            {
                ViewBag.Error = "No request was sent.";
                return View("UnsuccessfulUpload");
            }
            
        }

    }
}
