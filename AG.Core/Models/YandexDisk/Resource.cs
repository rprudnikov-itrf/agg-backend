using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models.YandexDisk
{
    public class Resource
    {
        public string public_key { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string public_url { get; set; }
        public string origin_path { get; set; }
        public string md5 { get; set; }
        public string mime_type { get; set; }
        public long size { get; set; }

        public DateTime created { get; set; }
        public DateTime modified { get; set; }

        public ResourceList _embedded { get; set; }
    }
}