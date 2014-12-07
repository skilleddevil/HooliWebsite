﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using Hooli.Models;
using Hooli.MySql;

namespace Hooli.Controllers
{
    public class CommentController : Controller
    {
        //
        // GET: /Comment/

        public ActionResult Index()
        {
            DBConnect db = new DBConnect();
            string query = "select * from comments where Software_ID = \"" + RouteData.Values["id"] + "\";";
            ViewBag.id = RouteData.Values["id"];
            MySqlCommand cmd = new MySqlCommand(query);
            var model = FillCommentModel(cmd);
            return View(model);
        }

        public ActionResult add()
        {
            string query = "select * from comments where id = \"" + RouteData.Values["id"] + "\";";
            MySqlCommand cmd = new MySqlCommand(query);
            var model = FillCommentModel(cmd);
            return View();
        }

        public ActionResult rate()
        {
            return View();
        }

        public ActionResult sort(FormCollection formCollection)
        {
            return View();
        }

        private IEnumerable<CommentModel> FillCommentModel(MySqlCommand cmd)
        {
            DBConnect db = new DBConnect();
            List<CommentModel> comments = new List<CommentModel>();
            if (db.GetData(cmd) != null)
            {
                foreach (DataRow row in db.GetData(cmd).Rows)
                {
                    comments.Add(new CommentModel()
                    {
                        ID = (int)row["ID"],
                        User_ID = (int)row["User_ID"],
                        User_Name = (String)row["User_Name"],
                        Software_ID = (int)row["Software_ID"],
                        Date = (DateTime)row["Date"],
                        Text = (string)row["Comment"],
                        Rating = (int)row["Rating"]
                    });
                }
            }
            IEnumerable<CommentModel> model = comments;
            return model;
        }
    }
}