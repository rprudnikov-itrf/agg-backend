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

           // НалоговаяВыгрузка.Run();
            //НалоговаяВыгрузка.RunFileReports();
            //НалоговаяВыгрузка.RunFileInvoices();

            //ПроверитьНомераДоговоров.Run();
            //ЗагрузитьСтарыеДокументы.Run();

             
            var date = new DateTime(2019, 1, 1);
            //СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date);
            ////СгенерироватьОтчетАгент.Run(Environment.CurrentDirectory, date, "bb53804f47c44d978c1e460e5cec63a8");
            //СгенерироватьОтчетВыводы.Run(Environment.CurrentDirectory, date);
            //РассылкаОтчетаАгента.Run(date);


            //СчетаФактуры.Run();
            //СгенерироватьДоговор.Run(Environment.CurrentDirectory);
            //СгенерироватьДоговор.Run("f5ff482cbc764672b93063d64f486a27", "68f1213c5825487b973f1e1279bf8421", Environment.CurrentDirectory, true);
            ЗагрузитьПлатежиКиви.Run();
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

//            СчетаЯндекс.Run(new DateTime(2019, 1, 1), @"
//Уфа	20190131011662	 167 086,48   
//Омск	20190131019931	 207 145,65   
//Челябинск	20190131012969	 681 661,13   
//Самара	20190131029105	 677 429,55  
//Москва	20190131010209	8 970 023,75
//Нижний Новгород	20190131012907	 167 687,46  
//Ростов-на-Дону	20190131017642	 719 239,36   
//Сочи	20190131002789	 56 852,63  
//Пермь	20190131003072	 1 009 960,31   
//Екатеринбург	20190131021690	 1 745 115,69   
//Казань	20190131003005	 1 187 745,00   
//Санкт-Петербург	20190131004320	 515 511,24   
//Новосибирск	20190131027238	 430 390,64   
//Тюмень	20190131029117	 695 228,23   
//Краснодар	20190131017718	 1 907 415,78   
//Саратов	20190131007297	 327 724,00   
//Ижевск	20190131007282	 139 697,60  
//Красноярск	20190131007278	 63 314,53   
//Воронеж	20190131021355	 55 273,81   
//Ставрополь	20190131005028	 45 319,62   
//Набережные Челны	20190131000832	 41 731,94   
//Курск	20190131025132	 1 332,01   
//Калининград	20190131003308	 146 534,17   
//Кемерово	20190131007274	 50 055,63  
//Чебоксары	20190131007319	 49 640,14   
//Хабаровск	20190131029099	 179 522,45   
//Владивосток	20190131029101	 169 302,77   ");


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
