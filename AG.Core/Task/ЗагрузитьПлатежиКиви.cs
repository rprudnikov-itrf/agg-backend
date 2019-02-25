using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace AG.Core.Task
{
    public static class ЗагрузитьПлатежиКиви
    {
        public static void Run()
        {
            var domains = AggregatorHelper.Client.List(true);
            var payment = new HashSet<string>()
            {
                "QIWI",
                "Qiwi",
                "КИВИ",
                "ISHOP_NEW",
                "ISHOP"
            };

            var culture = CultureInfo.GetCultureInfo("ru");
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            var lockobj = new Object();
            var totals = new Dictionary<string, double>();

            using (var log = File.CreateText(@"e:\qiwi.csv"))
            {
                for (var i = new DateTime(2016, 1, 1); i < new DateTime(2019, 1, 1); i = i.AddMonths(1))
                {
                    var end = new DateTime(i.Year, i.Month, DateTime.DaysInMonth(i.Year, i.Month));
                    var total = 0d;

                    Parallel.ForEach(domains, opt, domain =>
                    {
                        try
                        {
                            //if (!"682a7cf39169425aab48899880767b99".Equals(domain.Db))
                            //    return;

                            var transactions = AggregatorHelper.Transaction.List(domain.Agg, domain.Db, null, i, end)
                                .Where(p => p.factor == Models.AggregatorTransactionFactor.Приход && payment.Contains(p.pay_type))
                                .ToArray();

                            if (transactions.Count() == 0)
                                return;

                            lock (lockobj)
                            {
                                foreach (var item in transactions)
                                {
                                    //var sp = (item.description ?? "").Split(';').FirstOrDefault();
                                    if (string.IsNullOrEmpty(item.account_number))
                                        continue;

                                    if (!totals.ContainsKey(item.account_number))
                                        totals[item.account_number] = 0;

                                    totals[item.account_number] += item.sum;
                                    //Console.WriteLine(item.pay_type);
                                }

                                total += transactions.Sum(p => p.sum);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });

                    Console.WriteLine("{0:MM.yyyy} = {1:N2}", i, total);
                    log.WriteLine(string.Format(culture, "{0:dd.MM.yyyy};{1:F2}", i, total));
                }

                foreach (var item in totals)
                {
                    Console.WriteLine("{0} = {1:N2}", item.Key, item.Value);
                    log.WriteLine(string.Format(culture, "{0};{1:F2}", item.Key, item.Value));
                }
               
            }
        }
    }
}
