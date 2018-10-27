using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using System.Threading.Tasks;
using System.IO;
using AG.Core.Models;
using System.Web;

namespace AG.Core.Task
{
    public static class НайтиУКогоНетДоговоров
    {
        public static void Run()
        {
            var items = new List<string>();
            var domains = AggregatorHelper.Client.List(true)
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number))
                .OrderBy(p => p.Number)
                .ToArray();

            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
            Parallel.ForEach(domains, opt, domain =>
            {
                try
                {
                    var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, domain.Contract.Number);
                    var files = YandexDiskHelper.Folders.Resources(path);
                    var _files = new List<string>();
                    var count = 0;
                    if (files != null || files._embedded != null || files._embedded.items != null)
                    {
                        _files = files._embedded.items
                            .Where(p => !p.name.Contains("contract_"))
                            .Where(p => !p.name.Contains("_act_"))
                            .Where(p => !p.name.Contains("_invoice_"))
                            .Where(p => !p.name.Contains("_report2_"))
                            .Where(p => !p.name.Contains("dissolution_"))
                            .Select(p => p.name)
                            .ToList();

                        count = _files.Count();
                    }

                    items.Add(string.Join(",", 
                        domain.Number,
                        domain.Login,
                        domain.Company != null ? domain.Company.INN : "",
                        domain.Enable ? "Работает" : "Отключен",
                        count,
                        string.Join(",", _files)));

                    Console.WriteLine("{0} {1} {2}", count, path, string.Join(";", _files));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            File.WriteAllLines(@"e:\files.csv", items);
        }
    }
}
