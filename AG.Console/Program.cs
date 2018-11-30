using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.Core.Helpers;
using AG.Core.Models;
using AG.Core.Task;
using System.Globalization;

namespace AG
{
    class Program
    {
        static void Main(string[] args)
        {
            СверкаБанка.Run();
            //CSVДеталировка.Run("e55887936e564ee2a6a63470cca4c3a0", "c860d867d1d843cd9cdb467494ee87cd");
            //АрхивироватьОтчетыАгента.Run();

            //ОтчетПереключении.Run();
            //ОтчетОстатки2015.Run();
            //ОборотноСальдоваяВедомость.RunYear(Environment.CurrentDirectory);
            //ОборотноСальдоваяВедомость.Run(Environment.CurrentDirectory);

            //for (var i = new DateTime(2018, 8, 1); i <= new DateTime(2018, 8, 1); i = i.AddMonths(1))
            //{
            //    СформироватьСправки.Run(i, Environment.CurrentDirectory);
            //}

            //СверкаБанка.Run();

            //НалоговаяВыгрузка.Run();
            //НалоговаяВыгрузка.RunFileReports();
            //НалоговаяВыгрузка.RunFileInvoices();

            //ПроверитьНомераДоговоров.Run();
            //ЗагрузитьСтарыеДокументы.Run();


            var date = new DateTime(2018, 10, 1);
            //СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date);
            //СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date, "bb53804f47c44d978c1e460e5cec63a8");
            //СгенерироватьОтчетВыводы.Run(Environment.CurrentDirectory, date);
            //РассылкаОтчетаАгента.Run(date);


            //СчетаФактуры.Run();
            //СгенерироватьДоговор.Run(Environment.CurrentDirectory);
            //СгенерироватьДоговор.Run("f5ff482cbc764672b93063d64f486a27", "68f1213c5825487b973f1e1279bf8421", Environment.CurrentDirectory, true);
            //ЗагрузитьПлатежиКиви.Run();
            //ЗагрузитьОтчетПоКомиссии.Run(Environment.CurrentDirectory);
            //CSVСтатистика.Run();
            //НайтиУКогоНетДоговоров.Run();

            //СгенерироватьДоговор.Run("d1bf2d6baf30419f8addad3bb0ed1d7b", "f1e7bf84a5db4ab1bb888a95e96826eb", Environment.CurrentDirectory, true); //msk
            //СгенерироватьДоговор.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true); //spb
            //СгенерироватьДоговор.Run("971452918f1a497ba6c9fb71af2d0f21", "cff3acd3d66e4f72b87f860fa4747525", Environment.CurrentDirectory, true);
            //СгенерироватьРасторжение.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true);
            //ПроверитьРеквизиты.Run();
            //СверкаБанка.Run();


            //AggregatorHelper.Bill.Update(date);
            //СгенерироватьСчетаФактуры.Run(date);

//            СчетаЯндекс.Run(new DateTime(2018, 10, 1), @"
//Уфа	20181031000383	 299 559,89   
//Омск	20181031000320	 302 019,08   
//Челябинск	20181031000378	 637 295,79   
//Самара	20181031000336	 560 536,11   
//Москва	20181031003241	 14 860 516,78   
//Нижний Новгород	20181031000333	 348 691,82   
//Ростов-на-Дону	20181031000286	 906 008,19   
//Сочи	20181031000155	 104 510,86   
//Пермь	20181031019748	 1 204 728,50   
//Екатеринбург	20181031000324 	 1 987 693,77   
//Казань	20181031000212	 1 784 134,29   
//Санкт-Петербург	20181031000171	 819 087,48   
//Волгоград	20181031000305	 7 710,81
//Новосибирск	20181031000808	 652 070,37   
//Тюмень	20181031000541	 903 935,42   
//Краснодар	20181031000298	 2 118 309,11   
//Саратов	20181031001785	 390 319,38   
//Ижевск	20181031000958	 120 256,25   
//Новокузнецк	20181031001081	 12 722,38   
//Киров	20181031000933	 8 795,85   
//Красноярск	20181031001411	 90 642,24 
//Воронеж	20181031000327	 146 845,10   
//Ярославль	20181031000927	 4 614,97   
//Ставрополь	20181031002071	 134 594,92   
//Набережные Челны	20181031001348	 68 998,35   
//Курск	20181031003446	 112,83   
//Калининград	20181031019323	 131 834,66   
//Рязань	20181031001297	 4 136,79   
//Оренбург	20181031000323	 7 321,79   
//Кемерово	20181031000948	 64 805,62   
//Чебоксары	20181031000963	 177 161,16   
//Хабаровск	20181031000300	 99 127,48   
//Владивосток	20181031000313	 274 755,25   
//Пенза	20181031001084	 24 073,50");


            //Найти ИНН
            //var find = "131900992376";
            //foreach (var agg in AggregatorHelper.Aggegator.List())
            //{
            //    Console.WriteLine(agg.City);

            //    Parallel.For(0, 24, (i) =>
            //    {
            //        var date = (new DateTime(2015, 1, 1)).AddMonths(i);
            //        var reports = AggregatorHelper.Report.List(date, agg.Agg);
            //        if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
            //            return;

            //        var item = reports.Acts.FirstOrDefault(p => string.Equals(p.Value.inn, find, StringComparison.CurrentCultureIgnoreCase));
            //        if (item.Value != null)
            //        {
            //            Console.WriteLine(item.Key);
            //        }
            //    });
            //}

            //найти все ООО
            //var clients = AggregatorHelper.Client.List();
            //var count = clients.Where(p => p.Company != null && p.Enable && string.Equals(p.Company.OrgType, "ООО", StringComparison.CurrentCultureIgnoreCase)).Count();
            //if (count > 0)
            //{
            //}

            //посчитать разницу счетов Яндекс и фактической комиссии
            //var bill = 0d;
            //var act = 0d;
            //var aggs = AggregatorHelper.Aggegator.List().OrderBy(p => p.City).ToArray();

            //for (var i = new DateTime(2017, 1, 1); i < new DateTime(2018, 1, 1); i = i.AddMonths(1))
            //{


            //    foreach (var agg in aggs)
            //    {
            //        var reports = AggregatorHelper.Report.List(i, agg.Agg);
            //        if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
            //            continue;

            //        var calc = reports.Acts.Sum(p => p.Value.complete_commission_yandex);
            //        if (calc == 0)
            //            continue;

            //        if (reports.Bills == null)
            //        {
            //            Console.WriteLine("{0} - {1:d}", agg.Agg, i);
            //            continue;
            //        }

            //        var calc_bill = reports.Bills.Sum(p => p.Value.sum) + reports.Acts.Sum(p => p.Value.tips_pay);

            //        if (calc < calc_bill)
            //        {
            //            bill += calc_bill;
            //            act += calc;
            //        }
            //    }
            //}

            //var balance = act - bill;
            //if (balance > 0)
            //{
            //}

            Console.WriteLine("end");
            Console.Read();
        }
    }
}
