using AG.Core.Helpers;
using AG.Core.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AG.Core.Task
{
    public static class CSVДеталировка
    {
        public static void Run(string agg, string db, DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1).AddSeconds(-1);

            var item = AggregatorHelper.Client.Get(agg, db);
            var pay = AggregatorHelper.Pays.List(agg, db, start, end, true);
            using (var write = new StreamWriter(string.Format(@"E:\r_{0}_{1}.csv", item.City, item.Number), false, System.Text.Encoding.UTF8) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<AggregatorPayFullWriterMap>();
                csvWrite.WriteRecords(pay.Where(p => p.group == 16));
            }
        }
    }
}
