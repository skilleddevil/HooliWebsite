using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Hooli.Models
{
    public class SoftwareModel
    {
        public int id { get; set; }
        public int admin_id { get; set; }
        [Display(Name = "Software name")]
        [Required(ErrorMessage="The software's name is required")]
        public string softwareName { get; set; }
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "The software file is required")]
        [DataType(DataType.Upload)]
        //public string fileName { get; set; }
        public HttpPostedFileBase fileName { get; set; }
        [Display(Name="Version")]
        [Required(ErrorMessage="A version number is required")]
        public string version { get; set; }
        public DateTime date_added { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage="A description is required")]
        public string description { get; set; }
        public string data { get; set; }
        public string contentType { get; set; }
        public int downloads { get; set; }
        public int rating { get; set; }
    }

    public class SoftwareContext : DbContext
    {

        public SoftwareContext()
            : base("LocalMySqlServer")
        {
        }

        public DbSet<SoftwareModel> Software { get; set; }

    }
}