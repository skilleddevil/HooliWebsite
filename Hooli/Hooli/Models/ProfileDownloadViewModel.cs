using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hooli.Models
{
    public class ProfileDownloadViewModel
    {
        public ProfileModel profile { get; set; }
        public List<DownloadHistoryModel> history { get; set; }
    }
}