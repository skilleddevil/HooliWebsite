using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hooli.MySql;
using MySql.Data.MySqlClient;
using Hooli.Models;
using System.Data;
using System.Web.Security;

namespace Hooli.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index()
        {
            var user = Membership.GetUser();
            var userID = (int)user.ProviderUserKey;

            DBConnect db = new DBConnect();

            var profile_model = FillProfileModel(userID);
            var download_model = FillProfileDownloadViewModel(profile_model);

            return View(download_model);
        }

        private ProfileModel FillProfileModel(int userID)
        {
            string profile_query = "SELECT * FROM my_aspnet_users WHERE id = " + userID + ";";
            MySqlCommand profile_cmd = new MySqlCommand(profile_query);
            DBConnect db = new DBConnect();
            ProfileModel profile = new ProfileModel();
            if (db.GetData(profile_cmd) != null)
            {
                foreach (DataRow row in db.GetData(profile_cmd).Rows)
                {
                    profile.id = (int)row["id"];
                    profile.name = (string)row["name"];
                };

            }
            return profile;
        }

        private ProfileDownloadViewModel FillProfileDownloadViewModel(ProfileModel profile)
        {
            DBConnect db = new DBConnect();
            string download_query = "SELECT * FROM Download_history WHERE user_id = " + profile.id + ";";
            MySqlCommand download_cmd = new MySqlCommand(download_query);

            ProfileDownloadViewModel download = new ProfileDownloadViewModel();
            download.profile = profile;

            download.history = new List<DownloadHistoryModel>(1000);

            if (db.GetAffected(download_cmd) != 0)
            {
                foreach (DataRow row in db.GetData(download_cmd).Rows)
                {
                    DownloadHistoryModel model = new DownloadHistoryModel()
                    {
                        user_id = (int)row["user_id"],
                        softwareName = (string)row["softwareName"],
                        version = (string)row["version"],
                        id = (int)row["id"],
                        download_date = (DateTime)row["download_date"],
                        dl_version = (string)row["dl_version"]
                    };

                    download.history.Add(model);
                    /*download.history.Add(new DownloadHistoryModel()
                    {
                        user_id = (int)row["user_id"],
                        softwareName = (string)row["softwareName"],
                        version = (string)row["version"],
                        id = (int)row["id"],
                        download_date = (DateTime)row["download_date"],
                        dl_version = (string)row["dl_version"]
                    });*/
                }
            }


            return download;
        }




    }
}

