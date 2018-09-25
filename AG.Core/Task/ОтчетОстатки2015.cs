using AG.Core.Helpers;
using AG.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AG.Core.Task
{
    public static class ОтчетОстатки2015
    {
        public static void Run()
        {
            using (var write = new StreamWriter(@"E:\balance_2015.csv", false, System.Text.Encoding.UTF8) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                var disabled = AggregatorHelper.Client.Disabled();

                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<CsvItemMap>();

                foreach (var item in AggregatorHelper.Client.List(true).OrderBy(p => p.Number))
                {
                    if (!ЗагрузитьОтчетПоКомиссии.clients.Contains(item.Login))
                        continue;

                    if (item.Contract == null || item.Contract.Date == DateTime.MinValue)
                        continue;

                    var disable = disabled.Contains(item.Agg + ":" + item.Db);

                    var pay = AggregatorHelper.Pays.List(item.Agg, item.Db, item.Contract.Date.AddHours(-12), item.Contract.Date.AddMonths(1))
                        .OrderBy(p => p.date)
                        .Where(p => p.date >= item.Contract.Date.Date)
                        .FirstOrDefault();


                    csvWrite.WriteRecord(new CsvItem()
                    {
                        Status = disable ? "Отключен" : "Работает",
                        NumberContract = item.Contract != null ? item.Contract.Number : "",
                        DateContract = item.Contract != null ? item.Contract.Date : DateTime.MinValue,
                        Login = item.Login,
                        INN = item.Company != null ? item.Company.INN : "",
                        Company = item.Company != null ? item.Company.OrgName : "",
                        Balance = pay != null ? pay.balance - pay.sum_with_factor : 0d
                    });
                }
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
