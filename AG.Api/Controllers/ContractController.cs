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
    public class ContractController : Controller
    {
        //http://doc.rostaxi.info/contract?agg=d1bf2d6baf30419f8addad3bb0ed1d7b&db=1b06dd703adf4944b880dbeb891077a8
        public ActionResult Index(string agg, string db)
        {
            //return new HttpStatusCodeResult(404);

            var model = СгенерироватьДоговор.Run(agg, db, Server.MapPath("/bin"));
            if (model == null)
                return new HttpStatusCodeResult(404);

            return RedirectToAction(null, "download", new { file = model.public_key });
        }

        public ActionResult Tanker(string agg, string db, int month, int year)
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

            var reportCsv = reportsCsv[key];

            var БН = reportCsv.Возвраты_перечислений_парку
                            + reportCsv.Возвраты_прочие
                            + reportCsv.БН_заказы
                            + reportCsv.Компенсации
                            + reportCsv.Чаевые
                            - reportCsv.Заправки
                            - reportCsv.Штрафы_Я
                            - reportCsv.Возвраты_Пользователям
                            - reportCsv.Ручные_возвраты_техподдержкой;

            return Json(new
            {
                total = БН + reportCsv.Cубсидии + reportCsv.Купоны + reportCsv.Корпаративные_заказы,
                balance_end = reportCsv.Долг_АТ
            });
        }
    }
}