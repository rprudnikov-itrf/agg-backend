using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorPayResult
    {
        public AggregatorPayResult()
        {
            Items = new List<AggregatorPay>();
        }

        public List<AggregatorPay> Items { get; set; }

        public int PageCount { get; set; }

        public int Page { get; set; }
    }
}
