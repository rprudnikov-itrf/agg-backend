using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AG.Core.Helpers;

namespace AG.Core.Task
{
    public static class ПроверитьНомераДоговоров
    {
        public static void Run()
        {
            using (var file = File.CreateText("r:\\2.txt"))
            {
                var domains = AggregatorHelper.Client.List();
                var maxNumber = domains
                    .Where(p => p.Contract != null && !string.IsNullOrWhiteSpace(p.Contract.Number))
                    .Max(p => p.Contract.Number.TryParseInt32());

                foreach (var item in domains
                    .Where(p => p.Contract != null && !string.IsNullOrWhiteSpace(p.Contract.Number))
                    .OrderByDescending(p => p.Contract.Number.TryParseInt32())
                    .Take(5))
                {
                    Console.WriteLine("{0} {1:dd.MM.yyyy} = {2}", item.Contract.Number, item.Contract.Date, item.Login);
                }

                Console.WriteLine("************");

                foreach (var group in domains
                    .Where(p => p.Contract != null && !string.IsNullOrWhiteSpace(p.Contract.Number) &&  !p.Contract.Number.Contains('.'))
                    .GroupBy(p => p.Contract.Number.TryParseInt32())
                    .Where(p => p.Count() > 1)
                    .OrderBy(p => p.Key))
                {
                    Console.WriteLine(group.Key);

                    foreach (var item in group.OrderBy(p => p.Contract.Date).Skip(1))
                    {
                        Console.WriteLine("\t{0} {1:dd.MM.yyyy} = {2}", item.Contract.Number, item.Contract.Date, item.Login);

                        file.WriteLine("{0} {1} https://agg.taximeter.yandex.ru/db/{2}/{3}", ++maxNumber, item.Contract.Number, item.Agg, item.Db);
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("Max: {0}", maxNumber);
            }
        }
    }
}
