using AG.Core.Helpers;
using AG.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class ЗагрузкаАктовТанкер
    {
        public static void Run()
        {
            var clients = AggregatorHelper.Client.List(true);
            foreach (var dir in Directory.GetDirectories(@"E:\csv\tanker").OrderBy(p => p))
            {
                var number = new DirectoryInfo(dir).Name;
                Console.WriteLine(number);

                var client = clients.FirstOrDefault(p => p.Contract != null && p.Contract.Number == number);
                if (client == null)
                    continue;

                //foreach (var item in AggregatorHelper.File.List(client.Agg, client.Db))
                //{
                //    if (item.Value.Description != StaticHelper.GroupTanker)
                //        continue;

                //    AggregatorHelper.File.Delete(client.Agg, client.Db, item.Key);
                //}

                foreach (var file in Directory.GetFiles(dir, "*.pdf"))
                {
                    Console.WriteLine("\t{0}", file);

                    var date = Path.GetFileNameWithoutExtension(file).Split('_').LastOrDefault();
                    var dateTime = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), 1);

                    var aggregatorFile = new AggregatorFile()
                    {
                         Date = dateTime,
                         FileName = Path.GetFileName(file),
                         Group = "TANKER",
                         Description = StaticHelper.GroupTanker
                    };

                    var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, number);
                    if (!YandexDiskHelper.Folders.Exist(path))
                        YandexDiskHelper.Folders.Add(path);

                    var remoteFile = YandexDiskHelper.Folders.Combine(path, aggregatorFile.FileName);
                    var fileRaw = File.ReadAllBytes(file);
                    YandexDiskHelper.Files.Upload(remoteFile, fileRaw);

                    var resource = YandexDiskHelper.Share.Publish(remoteFile);
                    if (resource != null && !string.IsNullOrEmpty(resource.public_key))
                    {
                        aggregatorFile.FileUrl = StaticHelper.GenerateDownloadUrl(resource.public_key);
                        AggregatorHelper.File.Add(client.Agg, client.Db, aggregatorFile);
                    }
                }
            }
            
        }
    }
}
