using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorReport
    {
        public Dictionary<string, AggregatorAct> Acts { get; set; }
        public Dictionary<string, AggregatorBill> Bills { get; set; }
    }
}
