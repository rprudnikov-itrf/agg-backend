using AG.Core.Helpers;
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
    public static class ОтчетПереключении
    {
        public static void Run()
        {
            var records = new List<CsvItem>();
            var disabled = AggregatorHelper.Client.Disabled();
            var dates = new[] {
                    new DateTime(2017, 1, 1),
                    new DateTime(2017, 2, 1),
                    new DateTime(2017, 3, 1),
                    new DateTime(2017, 4, 1),
                    new DateTime(2017, 5, 1),
                    new DateTime(2017, 6, 1),
                    new DateTime(2017, 7, 1),
                    new DateTime(2017, 8, 1),
                    new DateTime(2017, 9, 1),
                    new DateTime(2017, 10, 1),
                    new DateTime(2017, 11, 1),
                    new DateTime(2017, 12, 1)
                };

            var reports = new List<AggregatorAct>();
            foreach (var agg in AggregatorHelper.Aggegator.List())
            {
                for (var i = new DateTime(2017, 1, 1); i < new DateTime(2018, 1, 1); i = i.AddMonths(1))
                {
                    var report = AggregatorHelper.Report.List(i, agg.Agg);
                    if (report != null && report.Acts != null)
                    {
                        reports.AddRange(report.Acts.Values);
                        Console.Write("*");
                    }
                }
            }

            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            var clients = AggregatorHelper.Client.List(true)
                .Where(p => p.Contract != null && p.Contract.Date > DateTime.MinValue.AddDays(1))
                .OrderBy(p => p.Contract.Date);

            Parallel.ForEach(clients, opt, item =>
            {
                try
                {
                    Console.Write(".");

                    var disable = disabled.Contains(item.Agg + ":" + item.Db);

                    var record = new CsvItem()
                    {
                        Status = disable ? "Отключен" : "Работает",
                        NumberContract = item.Contract != null ? item.Contract.Number : "",
                        DateContract = item.Contract != null ? item.Contract.Date : DateTime.MinValue,
                        Login = item.Login,
                        INN = item.Company != null ? item.Company.INN : "",
                        KPP = item.Company != null ? item.Company.KPP : "",
                        Company = item.Company != null ? item.Company.OrgName : "",
                        Comission1 = "2.0",
                        Comission2 = "2.0"
                    };

                    try
                    {
                        var lastPay = DateTime.MinValue;
                        if (item.Contract != null && item.Contract.Date > DateTime.MinValue.AddDays(1))
                        {
                            var emptyPay = false;
                            for (var i = item.Contract.Date.AddHours(-12); i < item.Contract.Date.AddMonths(1); i = i.AddHours(12))
                            {
                                var table = AggregatorHelper.Pays.List(item.Agg, item.Db, i, i.AddHours(12));
                                var pay = table
                                    .OrderBy(p => p.date)
                                    .Where(p => p.date >= item.Contract.Date.Date)
                                    .FirstOrDefault();

                                if (pay != null)
                                {
                                    record.DateFirstPay = pay.date;
                                    break;
                                }

                                if (!emptyPay)
                                {
                                    var pays = AggregatorHelper.Pays.List(item.Agg, item.Db, null, null);
                                    if (pays.Count() == 0)
                                        break;
                                }
                            }

                            if (disable)
                            {
                                var pay = AggregatorHelper.Pays.List(item.Agg, item.Db, null, null)
                                    .OrderByDescending(p => p.date)
                                    .Where(p => p.date >= item.Contract.Date.Date)
                                    .FirstOrDefault();

                                if (pay != null)
                                    record.DateEnd = pay.date.ToShortDateString();
                            }
                        }

                        if (item.City == "Москва" && record.DateContract != DateTime.MinValue && record.DateContract < new DateTime(2017, 5, 13))
                        {
                            record.Comission1 = "0.99; 1.49; 2.0";
                        }

                        if (record.DateContract < new DateTime(2018, 1, 1))
                        {
                            //база за 2017
                            record.Base = reports
                                .Where(p => p.agg == item.Agg && p.db == item.Db)
                                .Sum(p => p.complete_cost);

                            record.Comission = reports
                                .Where(p => p.agg == item.Agg && p.db == item.Db)
                                .Sum(p => p.complete_commission_rostaxi - p.cancel_commission_rostaxi);
                        }

                        records.Add(record);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                        record.Error = e.Message;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            using (var write = new StreamWriter(@"E:\switch_list.csv", false, System.Text.Encoding.UTF8) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<CsvItemMap>();
                csvWrite.WriteRecords(records);
            }
        }

        class CsvItem
        {
            public string Status { get; set; }
            public string NumberContract { get; set; }
            public DateTime DateContract { get; set; }
            public DateTime DateFirstPay { get; set; }
            public string DateEnd { get; set; }
            public string Login { get; set; }
            public string INN { get; set; }
            public string KPP { get; set; }
            public string Company { get; set; }
            public double Base { get; set; }
            public double Comission { get; set; }
            public string Comission1 { get; set; }
            public string Comission2 { get; set; }
            public string Error { get; set; }
        }

        class CsvItemMap : ClassMap<CsvItem>
        {
            public CsvItemMap()
            {
                Map(p => p.Status).Name("Статус");
                Map(p => p.NumberContract).Name("№ договора");
                Map(p => p.DateContract).Name("Дата договора").TypeConverterOption.Format("dd.MM.yyyy");
                Map(p => p.DateFirstPay).Name("Дата первого платежа").TypeConverterOption.Format("dd.MM.yyyy");
                Map(p => p.DateEnd).Name("Дата отключения");
                Map(p => p.Login).Name("Логин");
                Map(p => p.INN).Name("ИНН");
                Map(p => p.KPP).Name("КПП");
                Map(p => p.Company).Name("Компания");
                Map(p => p.Base).Name("База для расчета вознаграждения");
                Map(p => p.Comission).Name("Вознаграждение");
                Map(p => p.Comission1).Name("Процент до 12 мая 2017");
                Map(p => p.Comission2).Name("Процент с 13 мая 2017");
                Map(p => p.Error).Name("");
            }
        }
    }
}
