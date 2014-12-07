using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hooli.Models.ActiveSoftware
{
    public class ActiveSoftwareModel
    {
        [Display(Name="Value")]
        public string Value { get; set; }

        public List<string> ListOfItems { get; set; }
    }
}