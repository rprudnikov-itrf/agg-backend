using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using AG.Core.Models;
using System.IO;

namespace AG.Core.Task
{
    public static class CSVСтатистика
    {
        public static void Run()
        {
            //Run("comission.csv", p => p.complete_commission_rostaxi);
            //Run("card.csv", p => p.add_card_pay);
            //Run("corp.csv", p => p.corp_pay);
            Run("subsidies.csv", p => p.add_bonus_pay);
            //Run("coupon.csv", p => p.add_coupon_pay);
        }

        public static void Run(string filename, Func<AggregatorAct, double> func)
        {
            Console.WriteLine(filename);

            var file = new List<string>();
            var agg = AggregatorHelper.Aggegator.List().OrderBy(p => p.City);

            file.Add("Дата;" + string.Join(";", agg.Select(p => p.City)));

            for (var i = new DateTime(2016, 1, 1); i < new DateTime(2017, 1, 1); i = i.AddMonths(1))
            {
                Console.WriteLine("\t{0:MMMM yyyy}", i);

                var total = new List<double>();
                foreach (var item in agg)
                {
                    var act = AggregatorHelper.Report.List(i, item.Agg);
                    total.Add(act == null || act.Acts == null ? 0 : act.Acts.Values.Sum(func));
                }
                
                file.Add(i.ToString("MMMM yyyy") + ";" + string.Join(";", total.Select(p => p.ToString("F2"))));
            }

            File.WriteAllLines(@"e:\" + filename, file, Encoding.UTF8);
        }
    }
}
