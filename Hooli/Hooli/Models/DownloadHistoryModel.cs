using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hooli.Models
{
    public class DownloadHistoryModel
    {
        public int user_id { get; set; }
        public string softwareName { get; set; }
        public string version { get; set; }
        public int id { get; set; }
        public DateTime download_date { get; set; }
        public string dl_version { get; set; }

    }
}