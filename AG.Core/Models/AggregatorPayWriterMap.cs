using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Core.Models
{
    public sealed class AggregatorPayWriterMap : ClassMap<AggregatorPay>
    {
        public AggregatorPayWriterMap()
        {
            //Map(m => m.number).Name("number");
            //Map(m => m.date).Name("date");
            Map(m => m.pay_type).Name("pay_type");
            Map(m => m.group).Name("group");
            Map(m => m.sum).Name("sum").TypeConverterOption.CultureInfo(CultureInfo.GetCultureInfo("ru"));
            //Map(m => m.balance).Name("balance").TypeConverterOption(CultureInfo.GetCultureInfo("ru"));
            //Map(m => m.description).Name("description");
            //Map(m => m.id).Name("id");
        }
    }
}
