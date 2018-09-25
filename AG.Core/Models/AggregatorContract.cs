using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models
{
    public class AggregatorContract
    {
        public string Number { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

        //оригинал договора получен 
        public bool Signed { get; set; }
    }
}