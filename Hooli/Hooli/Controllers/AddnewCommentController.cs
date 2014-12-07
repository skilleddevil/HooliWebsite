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
    public class AddnewCommentController : Controller
    {
        //
        // GET: /AddComment/

        public ActionResult Index()
        {
            ViewBag.id = RouteData.Values["id"];
            return View();
        }
    
        [Authorize]
        public ActionResult Submit(FormCollection formCollection)
        {
            int rating_dummy_value = 3;
            int user_id = (int)Membership.GetUser().ProviderUserKey;
            String name = User.Identity.Name;
            
            String text = formCollection.Get("comment_text"), query;
            MySqlCommand cmd = new MySqlCommand();
            query = "insert into comments (Comment, User_ID, User_Name, Software_ID, Date, Rating) values (@text, @user_id, @name, @Software_ID, @date, @rating);";
            cmd.CommandText = query;
            cmd.Parameters.Add("@Software_ID", MySqlDbType.Int16).Value = formCollection.Get("id");
            cmd.Parameters.Add("@user_id", MySqlDbType.Int16).Value = user_id;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@text", MySqlDbType.String).Value = text;
            cmd.Parameters.Add("rating", MySqlDbType.Int16).Value = formCollection.Get("rating");
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            DBConnect db = new DBConnect();
            db.Insert(cmd);

            // Average new rating with current software rating
            /*
            String current_sw_rating = "select rating from Software where id = @id;";
            cmd.Parameters.Add("@id", MySqlDbType.Int16).Value = formCollection.Get("id");
            MySqlCommand current_software_rating = new MySqlCommand(current_sw_rating);
           
            int current_rating = (int) db.GetData(current_software_rating).Rows[0][0];
            int new_rating = (int) (.75 * current_rating + .25 * rating_dummy_value);
            String new_software_rating_query = "INSERT INTO Software (rating) VALUES (@rate);";
            cmd.Parameters.Add("@rate", MySqlDbType.Int16).Value = new_rating;
            MySqlCommand new_software_rating_cmd = new MySqlCommand(new_software_rating_query);
            db.Insert(new_software_rating_cmd);
            
             */

            return RedirectToAction("Index", "Comment", new {id = formCollection.Get("id") });
        }
    }
    
}
