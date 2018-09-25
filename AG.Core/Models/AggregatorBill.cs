using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorBill
    {
        public DateTime date { get; set; }
        public string number { get; set; }
        public double sum { get; set; }
        public string user { get; set; }

        public AggregatorBill()
        {
        }

        public AggregatorBill(string number, double sum)
        {
            this.number = number;
            this.sum = sum;
        }
    }
}
