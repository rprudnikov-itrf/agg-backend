using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;

namespace AG.Core.Task
{
    public static class ПроверитьРеквизиты
    {
        public static void Run()
        {
            var oRow = 0;
            var log = new List<string>();
            foreach (var item in AggregatorHelper.Client.List().Where(p => p.Enable).OrderBy(p => p.Login))
            {
                if (item.Company != null && !string.IsNullOrWhiteSpace(item.Company.OrgAccount) && item.Company.OrgAccount.Trim().StartsWith("408178"))
                {
                    var reports = AggregatorHelper.Report.Get(item.Agg, item.Db, new DateTime(2017, 1, 1), new DateTime(2017, 2, 1), new DateTime(2017, 3, 1));
                    var str = string.Format("{0}. {1} = {2} = {3:N0}", ++oRow, item.Login, item.Contract != null && item.Contract.Signed, reports.Sum(p => p.delete_balance));
                    Console.WriteLine(str);
                    log.Add(str);
                }
            }

            var x = string.Join(Environment.NewLine, log);
            if (x != null)
            {
            }
        }
    }
}
