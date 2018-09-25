using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace AG.Core.Models
{
    public class AggregatorClient : IEqualityComparer<AggregatorClient>
    {
        public string Agg { get; set; }
        public string Db { get; set; }

        public string Name { get; set; }
        public string City { get; set; }
        public string Login { get; set; }

        public bool Enable { get; set; }
        public double Balance { get; set; }

        public AggregatorCompany Company { get; set; }
        public AggregatorContract Contract { get; set; }

        public int Number
        {
            get
            {
                if (Contract == null || Contract.Number == null)
                    return int.MaxValue;

                var i = 0;
                var n = Regex.Replace(Contract.Number, "[^0-9]", "");

                if (Contract.Number.Contains("."))
                    n = Contract.Number.Substring(0, Contract.Number.IndexOf('.'));
                
                int.TryParse(n, out i);
                return i;
            }
        }

        public bool Equals(AggregatorClient x, AggregatorClient y)
        {
            return x.Agg == y.Agg && x.Db == y.Db;
        }

        public int GetHashCode(AggregatorClient obj)
        {
            return obj.Agg.GetHashCode() ^ obj.Db.GetHashCode();
        }
    }
}