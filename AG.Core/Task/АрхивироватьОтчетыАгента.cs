using AG.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class АрхивироватьОтчетыАгента
    {
        public static void Run()
        {
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            var clients = AggregatorHelper.Client.List(true)
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number))
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

                        var file = string.Format("{0}_act_", client.Login);
                        foreach (var item in files._embedded.items.Where(p => p.name.StartsWith(file)))
                        {
                            var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsArhivFolder, client.Contract.Number);
                            if (!YandexDiskHelper.Folders.Exist(path))
                                YandexDiskHelper.Folders.Add(path);

                            var _path = path + "/" + item.name;
                            YandexDiskHelper.Files.Rename(item.path, _path);
                        }
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
