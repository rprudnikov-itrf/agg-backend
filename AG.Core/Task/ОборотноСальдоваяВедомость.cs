using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Data;
using AG.Core.Models;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CsvHelper;

namespace AG.Core.Task
{
    public static class ОборотноСальдоваяВедомость
    {
        public static void Run(string reportPath)
        {
            var list = new List<BankItem>();
            foreach (var file in Directory.GetFiles(@"E:\csv", "*.csv"))
            {
                using (var reader = new StreamReader(file, System.Text.Encoding.GetEncoding(1251)))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.RegisterClassMap<BankItemMap>();
                    csv.Configuration.MissingFieldFound = null;
                    csv.Configuration.ReadingExceptionOccurred = null;

                    var result = csv.GetRecords<BankItem>().ToList();
                    if (result != null)
                        list.AddRange(result);
                }
            }

            for (var date = new DateTime(2018, 8, 1); date <= new DateTime(2018, 9, 1); date = date.AddMonths(1))
            {
                ОборотноСальдоваяВедомость.Run(date, Environment.CurrentDirectory, list);
            }
        }
        public static void Run(DateTime date, string reportPath, List<BankItem> bank)
        {
            Console.WriteLine(date.ToShortDateString());

            var payment = new HashSet<string>()
            {
                "QIWI",
                "Qiwi",
                "КИВИ",
                "ISHOP_NEW",
                "ISHOP"
            };
            var file = new List<string>();
            var clients = AggregatorHelper.Client.List(true);
            var reports = new List<AggregatorAct>();
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1).AddSeconds(-1);

            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            foreach (var agg in AggregatorHelper.Aggegator.List().OrderBy(p => p.City)) //.Take(10)
            {
                Console.WriteLine(agg.City);

                var report = AggregatorHelper.Report.List(date, agg.Agg);
                if (report == null || report.Acts == null)
                    continue;

                Parallel.ForEach(report.Acts, opt, item =>
                {
                    try
                    {
                        //if (item.Key != "96ebf41bb763415c858fb218ccea891d")
                        //    return;

                        //if (item.Value.total == 0)
                        //    return;

                        item.Value.db_city = agg.City;
                        item.Value.agg = agg.Agg;
                        item.Value.db = item.Key;

                        var client = clients.FirstOrDefault(p => string.Equals(p.Db, item.Key, StringComparison.CurrentCultureIgnoreCase));
                        if (client == null)
                            client = AggregatorHelper.Client.Get(agg.Agg, item.Key);

                        if (client != null)
                        {
                            item.Value.name = client.Name;

                            if (string.IsNullOrEmpty(item.Value.inn) && client.Company != null)
                                item.Value.inn = client.Company.INN;

                            if (client.Contract != null)
                                item.Value.db_number = client.Contract.Number;
                        }

                        var transactions = AggregatorHelper.Transaction.List(agg.Agg, item.Key, null, start, end)
                            .Where(p => p.factor == Models.AggregatorTransactionFactor.Приход && payment.Contains(p.pay_type))
                            .ToArray();

                        item.Value.qiwi_off = transactions.Sum(p => p.sum);

                        var payStart = AggregatorHelper.Pays.List(agg.Agg, item.Key, start.AddDays(-1), end)
                            .OrderBy(p => p.date)
                            .Where(p => p.date >= start)
                            .FirstOrDefault();

                        if (payStart != null)
                            item.Value.start_balance = payStart.balance - payStart.sum_with_factor;

                        var payEnd = AggregatorHelper.Pays.List(agg.Agg, item.Key, end.AddHours(-6), end.AddHours(6))
                            .OrderByDescending(p => p.date)
                            .Where(p => p.date <= end)
                            .FirstOrDefault();

                        if (payEnd != null)
                            item.Value.end_balance = payEnd.balance;

                        if (client != null && client.Company != null && client.Company.INN != null)
                        {
                            var bankItem = bank
                                .Where(p => string.Equals(p.ИННПолучателя, client.Company.INN, StringComparison.CurrentCultureIgnoreCase) && p.Сумма > 0)
                                .Where(p => p.Дата >= start && p.Дата <= end)
                                .ToArray();

                            item.Value.add_balance_bank = bankItem.Sum(p => p.Сумма);
                        }

                        Console.Write(".");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });

                reports.AddRange(report.Acts.Values.Where(p => !p.hide && !string.IsNullOrEmpty(p.db_number)));
            }

            //file.Add(";;;;;дебет;кредит;;;;;;;;;;дебет;кредит");
            file.Add("Город;ИНН;Договор;Наименование Принципала;логин парка;База;долг парка;долг АТ;Удержана комиссия Яндекс;Удержана комиссия АТ;Покупка смен;Заправки;Перечислено парку;Штрафы Я;Возвраты Пользователям;Ручные возвраты техподдержкой;Пополнения от Принципала;Пополнение от QIWI;Возвраты перечислений парку;Возвраты прочие;Б/Н заказы;Корпаративные заказы;Субсидии;Купоны;Компенсации;Чаевые;Долг парка;Долг АТ");

            //добавить отключенные парки
            foreach (var item in clients)
            {
                if (item.Contract != null && item.Contract.Date > end)
                    continue;

                if (item.Balance < 100 && item.Balance > -100)
                    continue;

                if (reports.Any(p => p.agg == item.Agg && p.db == item.Db))
                    continue;

                var report = new AggregatorAct()
                {
                    agg = item.Agg,
                    db = item.Db,
                    db_city = item.City,
                    db_name = item.Login,
                    name = item.Name,
                    start_balance = item.Balance,
                    end_balance = item.Balance,
                    db_number = item.Contract != null ? item.Contract.Number : ""
                };

                reports.Add(report);
            }

            foreach (var report in reports.OrderBy(p => p.Number).GroupBy(p=> p.Number))
            {
                var db = report.OrderByDescending(p => p.total).First();
                var delta = db.end_balance 
                    - (db.start_balance
                    - db.complete_commission_yandex
                    - db.complete_commission_rostaxi
                    - db.delete_balance
                    - db.cancel_commission_yandex
                    - db.cancel_commission_rostaxi
                    - db.delete_card_pay
                    - db.delete_hand_pay
                    + db.add_balance_bank
                    + db.qiwi_off
                    + (db.add_balance - db.qiwi_off - db.add_balance_bank)
                    + db.add_card_pay
                    + db.add_corp_pay
                    + db.compensation_pay
                    + db.add_bonus_pay
                    + db.add_coupon_pay
                    + db.tips_pay);

                //если остаток не верный ищем смены
                if (delta != 0)
                {
                    //смены
                    var pays = AggregatorHelper.Pays.Group(db.agg, db.db, start.AddHours(3), end.AddHours(3));
                    var dayPay = pays.Where(p => p.group == 13).Sum(p => p.sum_with_factor) * -1;
                    if (dayPay != 0)
                    {
                        db.complete_commission_yandex_day = dayPay;
                        delta += dayPay;
                    }

                    //TANKER
                    var tankerPay = pays.Where(p => p.group == 16).Sum(p => p.sum_with_factor) * -1;
                    if (tankerPay != 0)
                    {
                        db.tanker = tankerPay;
                        delta += tankerPay;
                    }

                    Console.Write("*");
                }

                file.Add(string.Join(";",
                    db.db_city,
                    db.inn,
                    db.db_number,
                    db.name,
                    db.db_name,
                    db.complete_cost,
                    db.start_balance < 0 ? Math.Abs(db.start_balance).ToString() : "",
                    db.start_balance > 0 ? db.start_balance.ToString() : "",
                    db.complete_commission_yandex.ToString(),
                    db.complete_commission_rostaxi.ToString(),
                    db.complete_commission_yandex_day.ToString(),
                    db.tanker.ToString(),
                    db.delete_balance.ToString(),
                    (db.cancel_commission_yandex + db.cancel_commission_rostaxi).ToString(),
                    db.delete_card_pay.ToString(),
                    db.delete_hand_pay.ToString(),
                    db.add_balance_bank.ToString(),
                    db.qiwi_off.ToString(),
                    (db.add_balance - db.qiwi_off - db.add_balance_bank).ToString(),
                    Math.Round(delta, 2),
                    db.add_card_pay.ToString(),
                    db.add_corp_pay.ToString(),
                    db.add_bonus_pay.ToString(),
                    db.add_coupon_pay.ToString(),
                    db.compensation_pay.ToString(),
                    db.tips_pay.ToString(),
                    db.end_balance < 0 ? Math.Abs(db.end_balance).ToString() : "",
                    db.end_balance > 0 ? db.end_balance.ToString() : ""
                ));
            }

            var text = string.Join(Environment.NewLine, file);
            File.WriteAllText(@"E:\csv\report\x_report_balance_" + date.ToString("yyyy_MM") + ".csv", text, Encoding.UTF8);
        }

        public static void RunYear(string reportPath)
        {
            var dateStart = new DateTime(2018, 1, 1);
            var report = new Dictionary<string, ReportItem>();
            for (var date = dateStart; date < dateStart.AddYears(1); date = date.AddMonths(1))
            {
                var file = string.Format(@"E:\csv\report\report_balance_{0}_{1:00}.csv", date.Year, date.Month);
                if (!File.Exists(file))
                    continue;

                Console.WriteLine(file);
                using (var reader = new StreamReader(file, System.Text.Encoding.UTF8))
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.RegisterClassMap<ReportItemMap>();
                    csv.Configuration.CultureInfo = System.Globalization.CultureInfo.CurrentCulture;

                    var result = csv.GetRecords<ReportItem>().ToList();
                    foreach (var item in result)
                    {
                        if (!report.ContainsKey(item.логин_парка))
                        {
                            report.Add(item.логин_парка, item);
                            continue;
                        }

                        report[item.логин_парка] = report[item.логин_парка] + item;
                    }
                }
            }

            var outfile = string.Format(@"E:\csv\report\report_balance_{0}.csv", dateStart.Year);
            using (var write = new StreamWriter(outfile, false, System.Text.Encoding.UTF8) { AutoFlush = true })
            using (var csvWrite = new CsvWriter(write))
            {
                csvWrite.Configuration.HasHeaderRecord = true;
                csvWrite.Configuration.Delimiter = ";";
                csvWrite.Configuration.RegisterClassMap<ReportItemMap>();
                csvWrite.WriteRecords(report);
            }
        }
    }
}
