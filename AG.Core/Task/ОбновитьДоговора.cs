using AG.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class ОбновитьДоговора
    {
        public static void Run()
        {
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            var clients = AggregatorHelper.Client.List(true)
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number) && p.Contract.Date < new DateTime(2017, 4, 1))
                .OrderBy(p => p.Number);
            Parallel.ForEach(clients, opt, client =>
            {
                try
                {
                    var remotePath = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, client.Contract.Number);
                    var files = YandexDiskHelper.Folders.Resources(remotePath);
                    if (files != null && files._embedded != null && files._embedded.items != null)
                    {
                        Console.WriteLine(client.Contract.Number);

                        var contract = files._embedded.items.Where(p => p.name.StartsWith("contract_"));
                        if (contract.Any(p => p.name.Contains("_arhiv")))
                            return;

                        foreach (var item in contract)
                        {
                            YandexDiskHelper.Files.Rename(item.path, item.path.Replace(".pdf", "_arhiv.pdf"));
                        }

                        СгенерироватьДоговор.Run(client.Agg, client.Db, Environment.CurrentDirectory);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }
    }
}
