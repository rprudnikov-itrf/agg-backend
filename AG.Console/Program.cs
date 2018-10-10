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
            CSVДеталировка.Run("e55887936e564ee2a6a63470cca4c3a0", "c860d867d1d843cd9cdb467494ee87cd");
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


            //var date = new DateTime(2018, 9, 1);
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

            //СгенерироватьДоговор.Run("d1bf2d6baf30419f8addad3bb0ed1d7b", "f1e7bf84a5db4ab1bb888a95e96826eb", Environment.CurrentDirectory, true); //msk
            //СгенерироватьДоговор.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true); //spb
            //СгенерироватьДоговор.Run("971452918f1a497ba6c9fb71af2d0f21", "cff3acd3d66e4f72b87f860fa4747525", Environment.CurrentDirectory, true);
            //СгенерироватьРасторжение.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true);
            //ПроверитьРеквизиты.Run();
            //СверкаБанка.Run();


            //AggregatorHelper.Bill.Update(new DateTime(2018, 8, 1));
            //СгенерироватьСчетаФактуры.Run(new DateTime(2018, 8, 1));

//            СчетаЯндекс.Run(new DateTime(2018, 8, 1), @"
//Уфа	20180831000359	 269 616,34   
//Омск	20180831000335	 380 298,72   
//Челябинск	20180831000394	 619 606,73   
//Самара	20180831000344	 491 276,33   
//Москва	20180831003308	 16 269 875,86   
//Нижний Новгород	20180831000326	 246 236,66   
//Ростов-на-Дону	20180831000295	 1 092 528,62   
//Сочи	20180831000157	 230 592,10   
//Пермь	20180831020447	 1 507 640,86   
//Екатеринбург	20180831000300	 2 167 318,18   
//Казань	20180831000211	 1 509 722,07   
//Санкт-Петербург	20180831000178	 902 859,62   
//Волгоград	20180831000322	 32 548,76   
//Новосибирск	20180831000810	 1 001 719,66   
//Тюмень	20180831000414	 1 042 642,23   
//Краснодар	20180831000303	 2 297 797,93   
//Саратов	20180831001806	 425 394,90   
//Ижевск	20180831000971	 183 343,28   
//Новокузнецк	20180831001113	 26 667,84   
//Киров	20180831000946	 31 103,68   
//Красноярск	20180831001414	 90 229,18  
//Воронеж	20180831000338	 105 252,16   
//Ярославль	20180831000942	 19 824,03 
//Ставрополь	20180831002084	 285 154,19   
//Набережные Челны	20180831001377	 218 178,94   
//Курск	20180831003508	 9 739,11   
//Калининград	20180831020181	 253 360,40   
//Рязань	20180831001302	 39 724,73
//Оренбург	20180831000330	 8 895,32   
//Кемерово	20180831000967	 83 577,89   
//Чебоксары	20180831000980	 305 473,17   
//Хабаровск	20180831000314	 72 122,36   
//Владивосток	20180831000319	 332 585,31  
//Пенза	20180831001106	 20 015,57");

//            СчетаЯндекс.Run(new DateTime(2018, 7, 1), @"
//Уфа	20180731003044	 299 870,04   
//Омск	20180731003012	395 251,52
//Челябинск	20180731003058	 743 723,31   
//Самара	20180731003023	 566 305,59   
//Москва	20180731006123	 17 324 089,27   
//Нижний Новгород	20180731003013	 248 242,86 
//Ростов-на-Дону	20180731002965	 1 137 020,06  
//Сочи	20180731002809	 219 087,49   
//Пермь	20180731002841	 1 530 570,37   
//Екатеринбург	20180731002949	 2 305 388,30   
//Казань	20180731002835	 1 573 815,87  
//Санкт-Петербург	20180731002825	 1 231 459,80   
//Волгоград	20180731003003	 46 242,09 
//Новосибирск	20180731003548	 878 699,24   
//Тюмень	20180731003082	 1 420 219,42   
//Краснодар	20180731002970	 2 460 142,86   
//Саратов	20180731003718	 594 055,61 
//Ижевск	20180731003711	 218 395,13   
//Новокузнецк	20180731003873	 30 104,28   
//Киров	20180731003682	 35 645,65 
//Красноярск	20180731003703	 99 241,72
//Воронеж	20180731003011	 128 942,93 
//Ярославль	20180731003675	 15 610,88
//Ставрополь	20180731004915	 218 279,58 
//Набережные Челны	20180731004170	 233 799,49 
//Курск	20180731006488	 19 247,02 
//Калининград	20180731004111	 236 941,34 
//Рязань	20180731004084	 16 443,71
//Оренбург	20180731003006	 9 112,73   
//Кемерово	20180731003706	 82 585,28 
//Чебоксары	20180731003714	 439 416,67   
//Хабаровск	20180731003005	 114 723,61   
//Владивосток	20180731003000	 273 214,41 
//Пенза	20180731003872	 21 161,93");

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
