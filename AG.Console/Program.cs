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
            //ЗагрузкаАктовТанкер.Run();
            //СверкаБанка.Run();
            //CSVДеталировка.Run("d1bf2d6baf30419f8addad3bb0ed1d7b", "f9e296addf9649258f018c8a2716158d", new DateTime(2017, 12, 1));
            //АрхивироватьОтчетыАгента.Run();

            //ОтчетПереключении.Run();
            //ОтчетОстатки2015.Run();
            //ОборотноСальдоваяВедомость.RunFinish();
            //ОборотноСальдоваяВедомость.RunYear(Environment.CurrentDirectory);
            //ОборотноСальдоваяВедомость.Run(Environment.CurrentDirectory);
            

            //for (var i = new DateTime(2018, 1, 1); i <= new DateTime(2018, 11, 1); i = i.AddMonths(1))
            //{
            //    //СформироватьСправки.Run(i, Environment.CurrentDirectory);
            //    СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, i);
            //}

            //СверкаБанка.Run();

            //НалоговаяВыгрузка.Run();
            //НалоговаяВыгрузка.RunFileReports();
            //НалоговаяВыгрузка.RunFileInvoices();

            //ПроверитьНомераДоговоров.Run();
            //ЗагрузитьСтарыеДокументы.Run();

             
            var date = new DateTime(2019, 2, 1);
            //СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date);
            ////СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date, "bb53804f47c44d978c1e460e5cec63a8");
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
            СгенерироватьРасторжение.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true);
            //ПроверитьРеквизиты.Run();
            //СверкаБанка.Run();


            //AggregatorHelper.Bill.Update(date);
            //СгенерироватьСчетаФактуры.Run(date);

//            СчетаЯндекс.Run(new DateTime(2019, 2, 1), @"
//Уфа	20190228000352	 143 338,11   
//Омск	20190228007369	 81 108,15   
//Челябинск	20190228000384	 381 453,03   
//Самара	20190228005831	 596 241,48   
//Москва	20190228016134	 8 511 236,04   
//Нижний Новгород	20190228021090	 143 141,90   
//Ростов-на-Дону	20190228005847	 634 284,47  
//Сочи	20190228013303	 31 782,00   
//Пермь	20190228022448	 799 625,93   
//Екатеринбург	20190228016114	1 515 262,92
//Казань	20190228015991	 658 283,21
//Санкт-Петербург	20190228015973	 240 297,08  
//Новосибирск	20190228013362	 363 238,66   
//Тюмень	20190228013514	 599 387,09   
//Краснодар	20190228007507	1 739 891,22
//Саратов	20190228014005	 79 865,68   
//Ижевск	20190228020781	 37 172,02   
//Красноярск	20190228020768	 39 725,21   
//Воронеж	20190228015922	 30 165,64  
//Ставрополь	20190228000282	 61 153,19   
//Набережные Челны	20190228007367	 57 469,53   
//Курск	20190228002637	 1 253,04   
//Калининград	20190228028974	 149 687,46 
//Кемерово	20190228011783	 44 981,24   
//Чебоксары	20190228013991	 59 233,70   
//Хабаровск	20190228021073	 165 230,44   
//Владивосток	20190228007891	 35 448,22");


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
