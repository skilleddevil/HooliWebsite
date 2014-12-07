using Hooli.Models;
using Hooli.MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hooli.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ActiveSoftwareController ctrl = new ActiveSoftwareController();

            DBConnect db = new DBConnect();
            string query = "select * from Software";
            MySqlCommand cmd = new MySqlCommand(query);
            var model = ctrl.FillSoftwareModel(cmd);
            var ordered = model.OrderByDescending(s => s.date_added).ToList();
            List<SoftwareModel> sendModel = new List<SoftwareModel>(3);
            for (int i = 0; i < 3; i++)
            {
                sendModel.Add(ordered[i]);
            } 
            IEnumerable<SoftwareModel> sendMe = sendModel;
                return View(sendMe);
        }
        
    }
}
