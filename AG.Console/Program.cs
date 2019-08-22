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


            var date = new DateTime(2019, 7, 1);
            ОборотноСальдоваяВедомость.Run(Environment.CurrentDirectory, date);
            СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date);
            //СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date, "642b4f4db5e049d5ac0b6794d9cb8765");
            СгенерироватьОтчетВыводы.Run(Environment.CurrentDirectory, date);
            РассылкаОтчетаАгента.Run(date);
            


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

//            СчетаЯндекс.Run(new DateTime(2019, 6, 1), @"
//Уфа	20190630001232	 64 265,82   
//Омск	20190630001230	 49 676,24   
//Челябинск	20190630001243 	 157 260,66   
//Самара	20190630001246	 76 428,56   
//Москва	20190630002246 	 5 046 560,38   
//Нижний Новгород	20190630001212 	 37 631,14   
//Ростов-на-Дону	20190630001253 	 54 807,71   
//Пермь	20190630010076	 252 571,56   
//Екатеринбург	20190630001319	 645 260,75   
//Казань	20190630001524	 118 339,06   
//Санкт-Петербург	20190630001355	 176 880,41   
//Новосибирск	20190630001127 	 261 524,26   
//Тюмень	20190630001298	362 006,94
//Краснодар	20190630001276	 812 279,12   
//Ижевск	20190630000215	 9 869,57   
//Красноярск	20190630000781	 3 794,24  
//Воронеж	20190630001180	 446,95   
//Курск	20190630003405	 114,49   
//Кемерово	20190630000212 	 1 311,90   
//Чебоксары	20190630000258 	 172,31   
//Хабаровск	20190630001160 	 162 874,85   
//Владивосток	20190630001161	 15 088,78   ");


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
