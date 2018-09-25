using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Threading;
using AG.Core.Helpers;
using AG.Core.Models;

namespace AG
{
    public static class СверкаБанка
    {
        public static void Run()
        {
            using (var write = new StreamWriter(@"r:\log.csv", false, System.Text.Encoding.GetEncoding(1251)) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<BankItemWriterMap>();

                var list = new List<BankItem>();
                foreach (var file in Directory.GetFiles(@"E:\csv", "*.csv"))
                {
                    using (var reader = new StreamReader(file, System.Text.Encoding.GetEncoding(1251)))
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = ";";
                        csv.Configuration.RegisterClassMap<BankItemMap>();

                        var result = csv.GetRecords<BankItem>().ToList();
                        if (result != null)
                            list.AddRange(result);
                    }
                }

                if (list != null)
                {
                    //Console.WriteLine("Поиск дубликатов переводов");

                    //var table = list
                    //    .Where(p => !"7710353606".Equals(p.ИННПолучателя))
                    //    .GroupBy(p => new { inn = p.ИННПолучателя, sum = p.Сумма, date = p.Дата.Date, desc = p.ОснованиеПлатежа })
                    //    .Where(p => p.Count() > 1);
                    //foreach (var group in table)
                    //{
                    //    Console.WriteLine("{0} = {1} = {2:d}", group.Key.inn, group.Count(), group.Key.date);

                    //    foreach (var item in group)
                    //    {
                    //        //Console.WriteLine("\t{0:N2} = {1}", item.Сумма, item.ОснованиеПлатежа);
                    //        csvWrite.WriteRecord(item);
                    //    }
                    //}

                    //Console.WriteLine("Total: {0} = {1}", table.Count(), table.SelectMany(p => p).Sum(p => p.Сумма));

                    //Console.WriteLine("Поиск неизвестных переводов");
                    //var clients = AggregatorHelper.Client.List();
                    //var inn = new HashSet<string>(clients.Where(p => p.Company != null).Select(p => (p.Company.INN ?? "").Trim()).Distinct());
                    //foreach (var item in list)
                    //{
                    //    var _inn = (item.ИННПолучателя ?? "").Trim();
                    //    if (inn.Contains(_inn)
                    //        || valid.Contains(_inn)
                    //        || "ИНН7735144097".Equals(item.НаименованиеПолучателя)
                    //        || "Вераксо Наталья Викторовна".Equals(item.НаименованиеПолучателя))
                    //        continue;

                    //    //Console.WriteLine("{0}; {1}; {2}; {3:N2}", item.ИННПолучателя, item.НаименованиеПолучателя, item.ОснованиеПлатежа, item.Сумма);
                    //    csvWrite.WriteRecord(item);
                    //}

                    //Console.WriteLine("Лидеры переводов");

                    //var table = list
                    //    .GroupBy(p => p.ИННПолучателя)
                    //    .OrderByDescending(p => p.Sum(x => Math.Abs(x.Сумма)))
                    //    .Take(100);

                    //foreach (var group in table)
                    //{
                    //    csvWrite.WriteRecord(new BankItem()
                    //    {
                    //        ИННПолучателя = group.Key,
                    //        НаименованиеПолучателя = group.First().НаименованиеПолучателя,
                    //        Сумма = Math.Round(group.Sum(p => Math.Abs(p.Сумма))),
                    //        Номер = group.Count()
                    //    });
                    //}


                    //Console.WriteLine("Переводы НВ");
                    //var table = list
                    //    .Where(p => p.НаименованиеПолучателя.ToUpper().Contains("ВЕРАКСО") || "ИНН7735144097".Equals(p.НаименованиеПолучателя))
                    //    .OrderBy(p => p.Дата);
                    //foreach (var item in table)
                    //{
                    //    csvWrite.WriteRecord(item);
                    //}
                }
            }
        }

        private static HashSet<string> valid = new HashSet<string>()
        {
            "7735507657",
            "7723137592",
            "7735144097",
            "7812014560",
            "7710353606",
            "503611767987",
            "7735071603",
            "7710030933",
            "7703363868",
            "7736207543",
            "7704340310",
            "5021020568",
            "711401540145",
            "5036140219",
            "5050081886",
            "7715999288",
            "5024124734",
            "7703707413",
            "7733738426",
            "5029198666",
            "7705902057",
            "5036146517",
            "7838049254",
            "7720327179",
            "5047043496",
            "7106041190",
            "7709678550",
            "7714409546",
            "7720667270",
            "5042134622",
            "5036087371",
            "7702372845",
            "7717107991",
            "772776193806",
            "7714807561",
            "7723863114",
            "7729365193",
            "5030038850",
            "5018182230",
            "7743128290",
            "5029215470",
            "7704521387",
            "7722856629",
            "7743947690",
            "1657231341",
            "7722333900",
            "7708739747",
            "7724761845",
            "5050109147",
            "7714868116",
            "7710382491",
            "7704211201",
            "7724311042",
            "7724686475",
            "772629634619",
            "773501283711",
            "5032190681",
            "5036084130",
            "7731293323",
            "594600083744",
            "5044057162",
            "5029149130",
            "503609104703",
            "5032055280",
            "772879403460",
            "5029192946",
            "7715469058",
            "774316636107",
            "5012082230",
            "7702840155",
            "7730702809",
            "402574120979",
            "770901719041",
            "860603489409",
            "3701048253",
            "7704336507",
            "5031071272",
            "5902300033",
            "505396576879",
            "501712090828",
            "0",
            "5905040465",
            "6608003052",
            "890403398626",
            "7721280413",
            "6671416456",
            "772506576396"
        };
    }
}
