using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorTransaction
    {
        public string id { get; set; }
        public string agg { get; set; }
        public string db { get; set; }
        public int number { get; set; }
        public DateTime date { get; set; }

        public string account_number { get; set; }
        public string transactions_number { get; set; }
        public string pay_type { get; set; }

        public AggregatorTransactionFactor factor { get; set; }
        public double sum { get; set; }
        public double sum_with_factor
        {
            get
            {
                return sum * (double)factor;
            }
        }
        public string description { get; set; }
    }
}
