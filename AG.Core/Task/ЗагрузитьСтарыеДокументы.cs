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
    public static class ЗагрузитьСтарыеДокументы
    {
        public static void Run()
        {
            var domains = AggregatorHelper.Client.List()
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number))
                .OrderBy(p => p.Contract.Number.TryParseInt32())
                .ToArray();

            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            Parallel.ForEach(domains, opt, domain =>
            {
                //if (!string.Equals(domain.Db, "a81f6f748bbc423c9bb95058a2065e1e"))
                //    return;

                try
                {
                    var documents = AggregatorHelper.File.List(domain.Agg, domain.Db)
                        .Where(p => p.Value.Date < new DateTime(2017, 5, 1))
                        .ToDictionary(p => p.Key, e => e.Value);

                    if (documents.Count == 0)
                        return;

                    var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, domain.Contract.Number);
                    var files = YandexDiskHelper.Folders.Resources(path);
                    if (files == null || files._embedded == null || files._embedded.items == null)
                    {
                        Console.WriteLine("Not Found {0}", path);
                        return;
                    }

                    Console.WriteLine("{0} {1}", files._embedded.items.Count, path);

                    foreach (var item in documents)
                    {
                        var file = files._embedded.items.FirstOrDefault(p => string.Equals(p.name, item.Value.FileName, StringComparison.CurrentCultureIgnoreCase));
                        if (file == null)
                        {
                            Console.WriteLine("File not found {0}", item.Value.FileName);
                            continue;
                        }

                        var share = YandexDiskHelper.Share.Publish(file.path);
                        if (share == null || string.IsNullOrEmpty(share.public_key))
                        {
                            Console.WriteLine("File share {0} error", item.Value.FileName);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(item.Value.FileUrl) && item.Value.FileUrl.Contains(HttpUtility.UrlEncode(share.public_key)))
                            continue;

                        item.Value.FileUrl = StaticHelper.GenerateDownloadUrl(share.public_key);
                        AggregatorHelper.File.Add(domain.Agg, domain.Db, item.Key, item.Value); 
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
