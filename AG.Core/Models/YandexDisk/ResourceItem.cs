using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models.YandexDisk
{
    public class ResourceItem
    {
        public string name { get; set; }
        public string preview { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string path { get; set; }
        public string md5 { get; set; }
        public string type { get; set; }
        public string mime_type { get; set; }
        public long size { get; set; }
    }
}