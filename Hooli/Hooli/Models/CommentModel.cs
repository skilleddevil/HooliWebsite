using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Hooli.Models
{
    public class CommentModel
    {
        public int ID { get; set; }
        public int User_ID { get; set; }
        public string User_Name { get; set; }
        public int Software_ID { get; set;  }
        public DateTime Date {get; set; }
        public string Text {get; set; }
        public int Rating { get; set; }
    }
}
