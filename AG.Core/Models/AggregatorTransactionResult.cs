using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorTransactionResult
    {
        public AggregatorTransactionResult()
        {
            Items = new List<AggregatorTransaction>();
        }

        public List<AggregatorTransaction> Items { get; set; }

        public int PageCount { get; set; }

        public int Page { get; set; }
    }
}
