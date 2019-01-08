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
            ОборотноСальдоваяВедомость.RunYear(Environment.CurrentDirectory);
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

             
            var date = new DateTime(2018, 12, 1);
            СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date);
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
            //СгенерироватьРасторжение.Run("971452918f1a497ba6c9fb71af2d0f21", "85d064616f62484c85aac4afc6769229", Environment.CurrentDirectory, true);
            //ПроверитьРеквизиты.Run();
            //СверкаБанка.Run();


            //AggregatorHelper.Bill.Update(date);
            //СгенерироватьСчетаФактуры.Run(date);

//            СчетаЯндекс.Run(new DateTime(2018, 11, 1), @"
//Уфа	20181130000335	 346 521,10   
//Омск	20181130000322	 257 767,75   
//Челябинск	20181130000337	 651 114,14  
//Самара	20181130000325	 634 141,83   
//Москва	20181130004071	 13 895 628,42   
//Нижний Новгород	20181130000316	 151 786,74   
//Ростов-на-Дону	20181130000279	 882 103,37   
//Сочи	20181130000152	 62 673,84   
//Пермь	20181130000163	 1 140 931,38   
//Екатеринбург	20181130000243	 1 733 061,72   
//Казань	20181130000156	 1 756 452,97   
//Санкт-Петербург	20181130000144	 663 764,76 
//Новосибирск	20181130000802	 671 720,02   
//Тюмень	20181130000321	 816 563,76   
//Краснодар	20181130000281	 2 195 120,73   
//Саратов	20181130000903	 472 147,94   
//Ижевск	20181130000889	 68 951,91 
//Красноярск	20181130000887	 99 775,17   
//Воронеж	20181130000329	 90 154,28 
//Ставрополь	20181130001880	 97 080,52   
//Набережные Челны	20181130001268	 43 291,56   
//Курск	20181130003107	 57,39   
//Калининград	20181130001202	 100 116,94 
//Кемерово	20181130000883	 49 663,10   
//Чебоксары	20181130000917	 200 044,86   
//Хабаровск	20181130000319	 103 028,58   
//Владивосток	20181130000327	 290 760,56   
//Пенза	20181130001014	 3 784,43   ");


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
