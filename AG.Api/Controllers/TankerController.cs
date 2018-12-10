using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using AG.Core.Helpers;
using AG.Core.Models;
using AG.Core.Task;
using CsvHelper;

namespace AG.Api.Controllers
{
    public class TankerController : Controller
    {
        public ActionResult Index(string agg, string db, int month, int year)
        {
            var domain = AggregatorHelper.Client.Get(agg, db);
            if(domain == null)
                return new HttpStatusCodeResult(404);

            var file = string.Format(@"E:\csv\report\report_balance_{0}_{1:00}.csv", year, month);
            if (!System.IO.File.Exists(file))
                return new HttpStatusCodeResult(404);

            var reportsCsv = new Dictionary<string, ReportItem>();
            using (var reader = new StreamReader(file, System.Text.Encoding.UTF8))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ";";
                csv.Configuration.RegisterClassMap<ReportItemMap>();
                csv.Configuration.CultureInfo = System.Globalization.CultureInfo.CurrentCulture;
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.ReadingExceptionOccurred = null;
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.BadDataFound = null;

                var result = csv.GetRecords<ReportItem>().ToList();
                foreach (var item in result)
                {
                    reportsCsv.Add(item.Город + "_" + item.логин_парка, item);
                }
            }

            var key = domain.City + "_" + domain.Login;
            if (!reportsCsv.ContainsKey(key))
                return new HttpStatusCodeResult(400);

            var report = reportsCsv[key];

            var cost1 = report.Возвраты_перечислений_парку
                     + report.Возвраты_прочие
                     + report.БН_заказы
                     + report.Компенсации
                     + report.Чаевые
                     - report.Штрафы_Я
                     - report.Возвраты_Пользователям
                     - report.Ручные_возвраты_техподдержкой;

            var resutl = new ReportCsv()
            {
                BalanceStart = report.долг_АТ,
                BalanceEnd = report.Долг_АТ,
                Cost1 = cost1,
                Cost2 = report.Корпаративные_заказы,
                Cost3 = report.Cубсидии + report.Купоны,
                Cost4 = report.Удержана_комиссия_Яндекс + report.Покупка_смен,
                Cost5 = report.Удержана_комиссия_АТ,
                Cost6 = report.Перечислено_парку
            };

            resutl.TotalCost = resutl.BalanceStart + resutl.Cost1 + resutl.Cost2 + resutl.Cost3
                - resutl.Cost4 - resutl.Cost5 - resutl.Cost6;

            return Json(resutl, JsonRequestBehavior.AllowGet);
        }
    }

    public class ReportCsv
    {
        public double BalanceStart { get; set; }
        public double BalanceEnd { get; set; }
        public double TotalCost { get; set; }

        public double Cost1 { get; set; }
        public double Cost2 { get; set; }
        public double Cost3 { get; set; }
        public double Cost4 { get; set; }
        public double Cost5 { get; set; }
        public double Cost6 { get; set; }
    }
}