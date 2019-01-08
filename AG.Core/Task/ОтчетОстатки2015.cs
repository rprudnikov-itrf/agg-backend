﻿using AG.Core.Helpers;
using AG.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class ОтчетОстатки2015
    {
        public static void Run()
        {
            var report = new List<AggregatorAct>();
            var aggs = AggregatorHelper.Aggegator.List();
            for (var i = new DateTime(2015, 9, 1); i < DateTime.Today; i = i.AddMonths(1))
            {
                Console.WriteLine(i.ToShortDateString());

                foreach (var agg in aggs)
                {
                    var items = AggregatorHelper.Report.List(i, agg.Agg);
                    if (items != null && items.Acts != null)
                    {
                        items.Acts.Values.ToList().ForEach(a =>
                        {
                            a.agg = agg.Agg;
                        });

                        report.AddRange(items.Acts.Values);
                    }
                }
            }

            var lockObj = new object();
            var csvItems = new List<CsvItem>();

            using (var write = new StreamWriter(@"E:\balance_2015.csv", false, System.Text.Encoding.UTF8) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                var disabled = AggregatorHelper.Client.Disabled();

                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<CsvItemMap>();

                var clients = AggregatorHelper.Client.List(true).OrderBy(p => p.Number);
                var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
                Parallel.ForEach(clients, opt, item =>
                {
                    Console.Write(".");
                    //if (!ЗагрузитьОтчетПоКомиссии.clients.Contains(item.Login))
                    //    continue;

                    //if (item.Contract == null || item.Contract.Date == DateTime.MinValue)
                    //    continue;

                    var disable = disabled.Contains(item.Agg + ":" + item.Db);

                    var pay = AggregatorHelper.Pays.List(item.Agg, item.Db, null, null)
                        .OrderByDescending(p => p.date)
                        //.Where(p => p.date >= item.Contract.Date.Date)
                        .FirstOrDefault();

                    var reports = report.Where(p => p.agg == item.Agg && p.db == item.Db);

                    lock (lockObj)
                    {
                        csvItems.Add(new CsvItem()
                        {
                            Status = disable ? "Отключен" : "Работает",
                            NumberContract = item.Contract != null ? item.Contract.Number : "",
                            DateContract = item.Contract != null ? item.Contract.Date : DateTime.MinValue,
                            Login = item.Login,
                            INN = item.Company != null ? item.Company.INN : "",
                            Company = item.Company != null ? item.Company.OrgName : "",
                            Balance = item.Balance,
                            DateLastPay = pay != null ? pay.date as DateTime? : null,
                            Null = reports.Sum(p => p.total) == 0
                        });
                    }
                });

                csvWrite.WriteRecords(csvItems.OrderBy(p => p.NumberContract));
            }
        }

        class CsvItem
        {
            public string Status { get; set; }
            public string NumberContract { get; set; }
            public DateTime DateContract { get; set; }
            public string Login { get; set; }
            public string INN { get; set; }
            public string Company { get; set; }

            public double Balance { get; set; }

            public bool Null { get; set; }

            public DateTime? DateLastPay { get; set; }
        }

        class CsvItemMap : ClassMap<CsvItem>
        {
            public CsvItemMap()
            {
                AutoMap();
                Map(p => p.DateContract).TypeConverterOption.Format("dd.MM.yyyy");
            }
        }
    }
}
