using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models
{
    public class AggregatorFile
    {
        public DateTime Date { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }

        public override string ToString()
        {
            return Date.ToString() + " - " + FileName;
        }
    }
}