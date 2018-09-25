using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Models;
using AG.Core.Helpers;

namespace AG.Core.Task
{
    public static class СчетаЯндекс
    {
        public static void RunTest(DateTime date, string bill)
        {
            Run(date, true, bill);
        }

        public static void Run(DateTime date, string bill)
        {
            Run(date, false, bill);
        }

        private static void Run(DateTime date, bool test, string bill)
        {
            var items = new Dictionary<string, AggregatorBill>();
            foreach (var item in bill.Trim().Split('\r'))
            {
                var token = item.Split('\t');
                if (token.Count() != 3)
                {
                    Console.WriteLine("Parse error: {0}", item);
                    return;
                }

                items.Add(token.ElementAt(0).Trim(), new AggregatorBill(token.ElementAt(1).Trim(), Convert.ToDouble(token.ElementAt(2).Replace(" ", "").Trim())));
            }

            Run(date, test, items);
        }

        public static void Run(DateTime date, bool test, Dictionary<string, AggregatorBill> bill)
        {
            var agg = AggregatorHelper.Aggegator.List();

            foreach (var item in bill)
            {
                try
                {
                    var aggItem = agg.FirstOrDefault(p => string.Equals(p.City, item.Key));
                    if (aggItem == null)
                    {
                        Console.WriteLine("{0} = Not found", item.Key);
                        continue;
                    }

                    Console.WriteLine("{0} = {1} = {2:N2}", aggItem.City, item.Value.number, item.Value.sum);

                    if (!test)
                    {
                        AggregatorHelper.Bill.Remove(aggItem.Agg, item.Value.number);
                        AggregatorHelper.Bill.Set(aggItem.Agg, "Наталия", date, item.Value.number, item.Value.sum);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("-------------");
            Console.WriteLine("count: {0}", bill.Count);
            Console.WriteLine("total: {0:N2}", bill.Sum(p => p.Value.sum)); 
        }
    }
}
