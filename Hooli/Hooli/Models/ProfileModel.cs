using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Hooli.Models
{
    public class ProfileModel
    {
        public int id { get; set; }
        public string name { get; set; }

    }
}