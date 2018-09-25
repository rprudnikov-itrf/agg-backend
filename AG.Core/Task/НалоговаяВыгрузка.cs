using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using AG.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class НалоговаяВыгрузка
    {
        public static void Run()
        {
            var items = GetItems().GroupBy(p => new { p.agg, p.db });
            var output = new StringBuilder();
            output.AppendLine("Договор;Заключен;Оригинал;Город;ИНН;Компания;Логин;Таксопарк;Телефон;Email;Налогооблажение;Статус");

            foreach (var report in items)
            {   
                if (report.Sum(p => p.total) == 0)
                    continue;

                var item = AggregatorHelper.Client.Get(report.Key.agg, report.Key.db);
                Console.WriteLine("{0} = {1}", item.Number, report.Count());

                output.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12:F2};{13:F2};{14:F2};{15}",
                    item.Contract == null || item.Number == int.MaxValue ? "" : item.Contract.Number,
                    item.Contract == null || item.Contract.Date == DateTime.MinValue ? "" : item.Contract.Date.ToShortDateString(),
                    item.Contract != null && item.Contract.Signed ? "Получен" : "",
                    item.City,
                    item.Company == null ? "" : item.Company.INN,
                    item.Company == null ? "" : item.Company.OrgName,
                    item.Login,
                    item.Name,
                    item.Company == null ? "" : (item.Company.Phone ?? "").Replace(";", ","),
                    item.Company == null ? "" : (item.Company.Email ?? "").Replace(";", ","),
                    item.Company == null ? "" : item.Company.Nalog,
                    item.Enable ? "" : "Выключен",
                    report.Sum(p => p.complete_commission_yandex),
                    report.Sum(p => p.complete_commission_rostaxi),
                    report.Sum(p => p.corp_pay),
                    item == null ? "delete" : ""
                );
                output.AppendLine();
            }

            File.WriteAllText("r:\\docs.csv", output.ToString(), Encoding.UTF8);
        }

        //загрузить отчеты агента
        public static void RunFileReports()
        {
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 6 };
            var clients = YandexDiskHelper.Folders.Resources("/Рос.Такси/Контрагенты/");
            var dates = new[] { "201604", "201605", "201606" };

            Parallel.ForEach(clients._embedded.items.OrderBy(p => p.name), opt, client =>
            {
                try
                {
                    var files = YandexDiskHelper.Folders.Resources(client.path);
                    if (files._embedded != null && files._embedded.items != null)
                    {
                        foreach (var item in files._embedded.items.Where(p => p.name.Contains("_act")))
                        {
                            var date = dates.FirstOrDefault(p => item.name.Contains(p));
                            if (date == null)
                                continue;

                            var filename = item.name.Replace("_act", "").Replace("01.pdf", ".pdf");

                            var upload = string.Format("/Рос.Такси/Бухгалтерия АТ/налоговая проверка 2017/Отчеты агента/{0}/{1}_{2}", date, client.name, filename);
                            if (YandexDiskHelper.Folders.Exist(upload))
                            {
                                Console.WriteLine(item.name + "- exist");
                                continue;
                            }

                            var file = YandexDiskHelper.Files.Download(item.path);
                            YandexDiskHelper.Files.Upload(upload, file);
                            Console.WriteLine(item.name + "- ok");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }
        //public static void RunFileReports()
        //{
        //    var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
        //    var items = GetItems()
        //        .GroupBy(p => new { p.agg, p.db })
        //        .Where(p => p.Sum(x => x.total) > 0);

        //    Parallel.ForEach(items, opt, report =>
        //    {
        //        try
        //        {
        //            var client = AggregatorHelper.Client.Get(report.Key.agg, report.Key.db);
        //            Console.WriteLine("{0} = {1}", client.Number, report.Count());

        //            foreach (var item in report)
        //            {
        //                var number = client.Contract == null || string.IsNullOrEmpty(client.Contract.Number) ? "0" : client.Contract.Number;
        //                if (number == "0")
        //                {
        //                    Console.WriteLine("Error number - {0} {1}", client.Login, client.City);
        //                    continue;
        //                }

        //                var filename = string.Format("{0}_act_{1:yyyyMMdd}.pdf", client.Login, item.date);
        //                var path = string.Format("/Рос.Такси/Контрагенты/{0}/{1}", number, filename);
        //                var file = YandexDiskHelper.Files.Download(path);
        //                if (file == null || file.Length == 0)
        //                {
        //                    Console.WriteLine("\t" + filename + "- not found file");
        //                    continue;
        //                }

        //                var upload = string.Format("/Рос.Такси/Бухгалтерия АТ/налоговая проверка 2017/Отчеты агента/{2:yyyyMM}/{0}_{1}_{2:yyyyMM}.pdf", number, client.Login, item.date);
        //                YandexDiskHelper.Files.Upload(upload, file);
        //                Console.WriteLine("\t" + filename + "- ok");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }
        //    });
        //}

        //загрузить стеча-фактуры

        public static void RunFileInvoices()
        {
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 6 };
            var clients = YandexDiskHelper.Folders.Resources("/Рос.Такси/Контрагенты/");
            var dates = new[] { "201604", "201605", "201606" };

            Parallel.ForEach(clients._embedded.items.OrderBy(p => p.name), opt, client =>
            {
                try
                {
                    var files = YandexDiskHelper.Folders.Resources(client.path);
                    if (files._embedded != null && files._embedded.items != null)
                    {
                        foreach (var item in files._embedded.items.Where(p => p.name.Contains("_invoice")))
                        {
                            var date = dates.FirstOrDefault(p => item.name.Contains(p));
                            if (date == null)
                                continue;
                            
                            var upload = string.Format("/Рос.Такси/Бухгалтерия АТ/налоговая проверка 2017/Счета-фактуры/{0}/{1}_{2}", date, client.name, item.name);
                            if (YandexDiskHelper.Folders.Exist(upload))
                            {
                                Console.WriteLine(item.name + "- exist");
                                continue;
                            }

                            var file = YandexDiskHelper.Files.Download(item.path);
                            YandexDiskHelper.Files.Upload(upload, file);
                            Console.WriteLine(item.name + "- ok");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }

        private static IEnumerable<AggregatorAct> GetItems()
        {
            var aggs = AggregatorHelper.Aggegator.List();
            var items = new List<Dictionary<string, AggregatorAct>>();
            for (var i = new DateTime(2016, 4, 1); i <= new DateTime(2016, 6, 1); i = i.AddMonths(1))
            {
                Console.WriteLine(i.ToShortDateString());

                foreach (var agg in aggs)
                {
                    var reports = AggregatorHelper.Report.List(i, agg.Agg);
                    if (reports.Acts == null)
                        continue;

                    foreach (var item in reports.Acts)
                    {
                        item.Value.date = i;
                        item.Value.db = item.Key;
                        item.Value.agg = agg.Agg;
                    }

                    items.Add(reports.Acts);
                }
            }
            return items.SelectMany(p => p.Values);
        }
    }
}
