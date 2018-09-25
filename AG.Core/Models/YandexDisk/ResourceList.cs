using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models.YandexDisk
{
    public class ResourceList
    {
        public string sort { get; set; }
        public string path { get; set; }
        public List<ResourceItem> items { get; set; }
    }
}