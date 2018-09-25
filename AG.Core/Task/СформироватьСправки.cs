using AG.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace AG.Core.Task
{
    public static class СформироватьСправки
    {
        public static void Run(DateTime date, string reportPath, string db = "")
        {
            try
            {
                var baseDir = Path.Combine(@"E:\sp\", date.ToString("yyyy-MM"));
                if (Directory.Exists(baseDir))
                    Directory.Delete(baseDir, true);

                Directory.CreateDirectory(baseDir);
                Console.WriteLine(date.ToString());

                foreach (var agg in AggregatorHelper.Aggegator.List().OrderBy(p => p.City))
                {
                    var reports = AggregatorHelper.Report.List(date, agg.Agg);
                    if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
                        continue;

                    var ds = new DataSet();
                    var table = new System.Data.DataTable("p");
                    table.Columns.Add("name");
                    table.Columns.Add("value");
                    table.Rows.Add("city", agg.City);
                    table.Rows.Add("period", date.ToString("MMMM yyyy"));
                    table.Rows.Add("date", date.AddMonths(1).AddDays(-1).ToString("dd MMMM yyyy"));
                    ds.Tables.Add(table);

                    var tableItem = new System.Data.DataTable("r");
                    tableItem.Columns.Add("document_number");
                    tableItem.Columns.Add("org_type");
                    tableItem.Columns.Add("org_name");
                    tableItem.Columns.Add("org_inn");
                    tableItem.Columns.Add("marketing");
                    tableItem.Columns.Add("corp");
                    ds.Tables.Add(tableItem);

                    var total_marketing = 0.0;
                    var total_corp = 0.0;

                    var opt = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
                    Parallel.ForEach(reports.Acts.OrderBy(p => p.Value.db_name), opt, report =>
                    {
                        if (!string.IsNullOrWhiteSpace(db) && !string.Equals(report.Key, db, StringComparison.CurrentCultureIgnoreCase))
                            return;

                        Console.Write(".");

                        try
                        {
                            if (report.Value == null || report.Value.hide)
                                return;

                            var domain = AggregatorHelper.Client.Get(agg.Agg, report.Key);
                            if (domain == null || domain.Contract == null || string.IsNullOrEmpty(domain.Contract.Number) || domain.Company == null)
                                return;

                            var marketing = report.Value.add_bonus_pay;
                            if (domain.City != "Москва" && domain.City != "Санкт-Петербург")
                                marketing += report.Value.add_coupon_pay;

                            lock (tableItem)
                            {
                                tableItem.Rows.Add(
                                    domain.Contract.Number,
                                    domain.Company.OrgType,
                                    domain.Company.OrgName,
                                    domain.Company.INN,
                                    marketing.ToString("N2"),
                                    report.Value.corp_pay.ToString("N2"));
                            }

                            total_marketing += marketing;
                            total_corp += report.Value.corp_pay;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });

                    table.Rows.Add("total_marketing", total_marketing.ToString("N2"));
                    table.Rows.Add("total_corp", total_corp.ToString("N2"));

                    var xmlDoc = new XmlDataDocument(ds);
                    if (total_marketing > 0)
                        MakeReport(xmlDoc, Path.Combine(reportPath, "Resources", "sp_marketing.xml"), Path.Combine(baseDir, "marketing_" + agg.City + ".pdf"));

                    if (total_corp > 0)
                        MakeReport(xmlDoc, Path.Combine(reportPath, "Resources", "sp_corp.xml"), Path.Combine(baseDir, "corp_" + agg.City + ".pdf"));
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void MakeReport(XmlDataDocument xmlDoc, string xtFile, string outputFile)
        {
            try 
            { 
                var xt = new XslTransform();

                using (var ms = new MemoryStream())
                {
                    if (System.IO.File.Exists(xtFile))
                    {
                        xt.Load(xtFile);
                        xt.Transform(xmlDoc, null, ms);
                    }

                    var buffer = ms.ToArray();
                    if (buffer.Length == 0)
                        return;

                    var pdf = DocumentHelper.ConvetToPdf(buffer);
                    File.WriteAllBytes(outputFile, pdf);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
