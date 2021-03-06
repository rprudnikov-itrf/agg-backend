﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorPay
    {
        public string id { get; set; }
        public string agg { get; set; }
        public string db { get; set; }
        public int number { get; set; }
        public DateTime date { get; set; }

        public int group { get; set; }

        public string account_number { get; set; }
        public string transactions_number { get; set; }

        public string pay_type
        {
            get
            {
                if (string.IsNullOrEmpty(description))
                    return "";

                var token = description.IndexOf("№");
                if (token > 0) return description.Substring(0, token);
                return description;
            }
        }

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

        public double balance { get; set; }
    }
}
