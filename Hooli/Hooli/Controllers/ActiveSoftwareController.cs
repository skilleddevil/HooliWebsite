using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using Hooli.Models;
using Hooli.MySql;
using System.Web.Security;

namespace Hooli.Controllers
{
    public class ActiveSoftwareController : Controller
    {
        //
        // GET: /ActiveSoftware/

        public ActionResult Index()
        {
            DBConnect db = new DBConnect();
            string query = "select * from Software";
            MySqlCommand cmd = new MySqlCommand(query);
            var model = FillSoftwareModel(cmd);
            return View(model);
        }
        [Authorize]
        public FileResult Download()
        {
            DBConnect db = new DBConnect();
            string softwareId = (string)RouteData.Values["id"];
            string query = "select * from Software where id = " + softwareId + ";";
            MySqlCommand cmd = new MySqlCommand(query);
            DataTable dt = db.GetData(cmd);
            Byte[] bytes = (Byte[])dt.Rows[0]["data"];
            string contentType = (string)dt.Rows[0]["contentType"];
            string fileName = (string)dt.Rows[0]["fileName"];
            string softwareName = (string)dt.Rows[0]["softwareName"];
            string version = (string)dt.Rows[0]["version"];
            var user = Membership.GetUser();
            var userID = (int)user.ProviderUserKey;
            UpdateDownload(userID, softwareName, version, softwareId, version);

            return File(bytes, contentType, fileName);
        }

        private void UpdateDownloadCount(string id, int prevDownloads)
        {
            string query = "Update Software set downloads = @newDownloads where id = @id";
            DBConnect db = new DBConnect();
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.Add("@id", MySqlDbType.String).Value = id;
            cmd.Parameters.Add("@newDownloads", MySqlDbType.Int16).Value = prevDownloads + 1;
            db.Update(cmd);
        }

        [Authorize(Roles = "Admin")] 
        public ActionResult Delete()
        {
            if (Roles.IsUserInRole("Admin"))
            {
                string query = "Delete from Software where id = @softwareId";
                MySqlCommand cmd = new MySqlCommand(query);
                cmd.Parameters.Add("@softwareId", MySqlDbType.String).Value = (string)RouteData.Values["id"];

                DBConnect db = new DBConnect();
                db.Delete(cmd);

                query = "select * from Software";
                cmd.CommandText = query;
                var model = FillSoftwareModel(cmd);

                return View("Index", model);
            }
            return View("UnauthorizedAccess", "AddNewSoftware");
        }

        [Authorize(Roles = "Admin")] 
        public ActionResult Edit()
        {
            if (Roles.IsUserInRole("Admin"))
            {
                string query = "select * from Software where id = \"" + RouteData.Values["id"] + "\";";
                MySqlCommand cmd = new MySqlCommand(query);
                var model = FillSoftwareModel(cmd);
                if (model.Count() != 0) return View(model.ElementAt(0));
                else return View("Error");
            }
            return View("UnauthorizedAccess", "AddNewSoftware");
        }

        public ActionResult Update(SoftwareModel model)
        {
            string query = "Update Software set softwareName=@softwareName, version=@version, date_added=@date, " +
                           "description=@description where id = @id;";
            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@softwareName", model.softwareName);
            cmd.Parameters.AddWithValue("@version", model.version);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@description", model.description);
            cmd.Parameters.AddWithValue("@id", model.id);

            DBConnect db = new DBConnect();
            db.Update(cmd);

            query = "select * from Software";
            cmd.CommandText = query;
            var modelList = FillSoftwareModel(cmd);

            return View("Index", modelList);
        }

        public ActionResult Search(FormCollection formCollection)
        {
            String softwareName = formCollection.Get("Search_input"), query;
            MySqlCommand cmd = new MySqlCommand();
            var checkedButton = formCollection.Get("searchType");
            ViewBag.Search = true;
            if (checkedButton == "isExactly")
            {
                query = "select * from Software where softwareName = @softwareName";
                cmd.CommandText = query;
                cmd.Parameters.Add("@softwareName", MySqlDbType.String).Value = softwareName;
                var model = FillSoftwareModel(cmd);
                return View("Index", model);
                
            }
            else if (checkedButton == "contains")
            {
                List<SoftwareModel> software = new List<SoftwareModel>();
                query = "select * from Software";
                cmd.CommandText = query;
                var allSoftware = FillSoftwareModel(cmd);
                if (allSoftware.Count() != 0)
                {
                    foreach (SoftwareModel model in allSoftware)
                    {
                        if (model.softwareName.Contains(softwareName))
                        {
                            software.Add(model);
                        }
                    }
                    return View("Index", software);
                }
                else return View("Error");
            }
            else
            {
                return View("Error");
            }
        }

        public IEnumerable<SoftwareModel> FillSoftwareModel(MySqlCommand cmd)
        {
            DBConnect db = new DBConnect();
            List<SoftwareModel> software = new List<SoftwareModel>();
            if (db.GetData(cmd) != null)
            {
                foreach (DataRow row in db.GetData(cmd).Rows)
                {
                    software.Add(new SoftwareModel()
                    {
                        id = (int)row["id"],
                        admin_id = (int)row["admin_id"],
                        softwareName = (string)row["softwareName"],
                        version = (string)row["version"],
                        date_added = (DateTime)row["date_added"],
                        description = (string)row["description"],
                        downloads = (int)row["downloads"],
                        rating = (int)row["rating"]
                    });
                }
            }
            IEnumerable<SoftwareModel> model = software;
            return model;
        }

        public DownloadHistoryModel FillDownloadHistoryModel(MySqlCommand cmd)
        {
            DBConnect db = new DBConnect();
            DownloadHistoryModel download = new DownloadHistoryModel();
            if (db.GetData(cmd) != null)
            {
                if (db.GetData(cmd).Rows.Count == 0)
                {
                    return null;
                }
                foreach (DataRow row in db.GetData(cmd).Rows)
                {
                    download.user_id = (int)row["user_id"];
                    download.softwareName = (string)row["softwareName"];
                    download.version = (string)row["version"];
                    download.id = (int)row["id"];
                    download.download_date = (DateTime)row["download_data"];
                    download.dl_version = (string)row["dl_version"];
                }
            }
            return download;
        }

        private void UpdateDownload(int userID, string softwareName, string version, string softID, string dl_version)
        {
            DBConnect db = new DBConnect();
            string query = "select * from Download_history where user_id = @userid and softwareName = @softwareName;";

            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@userid", userID);
            cmd.Parameters.AddWithValue("@softwareName", softwareName);

            var queryResult = FillDownloadHistoryModel(cmd);


            string insert_query = "";
            if (queryResult == null)
            {
                insert_query = "insert into Download_history (user_id, softwareName, version, id, dl_version) values (@user_id, @softwareName, @version, @id, @dl_version);";
                cmd = new MySqlCommand(insert_query);
                cmd.Parameters.AddWithValue("@user_id", userID);
                cmd.Parameters.AddWithValue("@softwareName", softwareName);
                cmd.Parameters.AddWithValue("@version", version);
                cmd.Parameters.AddWithValue("@id", softID);
                cmd.Parameters.AddWithValue("@dl_version", dl_version);
            }
            else
            {
                insert_query = "update Download_history set version = @version, dl_version = @dl_version where user_id = @userID and softwareName = @softwareName;";
                cmd = new MySqlCommand(insert_query);
                cmd.Parameters.AddWithValue("@user_id", userID);
                cmd.Parameters.AddWithValue("@softwareName", softwareName);
                cmd.Parameters.AddWithValue("@version", version);
                cmd.Parameters.AddWithValue("@dl_version", dl_version);
            }

            db.Update(cmd);
        }
    }
}
