using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.Core.Helpers;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using AG.Core.Models;

namespace AG.Core.Task
{
    public static class СгенерироватьОтчетВыводы
    {
        public static void Run(string reportPath, DateTime date)
        {
            Run(reportPath, date, null);
        }

        public static void Run(string reportPath, DateTime date, string db)
        {
            Console.WriteLine(date.ToShortDateString());

            var sdate = new DateTime(date.Year, date.Month, 1);
            var edate = sdate.AddMonths(1).AddDays(-1);

            var aggs = AggregatorHelper.Aggegator.List();
            foreach (var agg in aggs)
            {
                var reports = AggregatorHelper.Report.List(date, agg.Agg);
                if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
                    continue;

                var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
                Parallel.ForEach(reports.Acts, opt, report =>
                {
                    if (!string.IsNullOrWhiteSpace(db) && !string.Equals(report.Key, db, StringComparison.CurrentCultureIgnoreCase))
                        return;

                    Console.Write(".");

                    try
                    {
                        if (report.Value == null || report.Value.delete_balance == 0)
                            return;

                        var items = AggregatorHelper.Transaction.List(agg.Agg, report.Key, string.Empty, sdate, edate)
                            .Where(p => p.factor == AggregatorTransactionFactor.Расход || (p.factor == AggregatorTransactionFactor.Приход && (p.description ?? "").ToLower().Contains("возврат")))
                            .ToList();

                        if (items.Count == 0)
                        {
                            items = new List<AggregatorTransaction>();
                            items.Add(new AggregatorTransaction()
                            {
                                date = date,
                                sum = Math.Max(report.Value.delete_balance + report.Value.delete_hand_pay, 0)
                            });
                        }

                        var domain = AggregatorHelper.Client.Get(agg.Agg, report.Key);
                        if (domain == null || domain.Contract == null || string.IsNullOrEmpty(domain.Contract.Number) || domain.Company == null)
                            return;

                        var ds = new DataSet();
                        var table = new System.Data.DataTable("p");
                        table.Columns.Add("name");
                        table.Columns.Add("value");
                        ds.Tables.Add(table);

                        table.Rows.Add("act_date", date.AddMonths(1).AddDays(-1).ToString("dd.MM.yyyy"));
                        table.Rows.Add("act_period", date.ToString("MMMM yyyy"));
                        table.Rows.Add("document_number", string.Format("{0}-{1:yyyy}", domain.Contract.Number, domain.Contract.Date));
                        table.Rows.Add("document_date", domain.Contract.Date.ToShortDateString());
                        table.Rows.Add("org_type", domain.Company.OrgType);
                        table.Rows.Add("org_name", domain.Company.OrgName);
                        table.Rows.Add("org_base", domain.Company.BaseDocument);
                        table.Rows.Add("org_face", domain.Company.FaceAcceptDocument);
                        table.Rows.Add("org_face_post", domain.Company.Post);
                        table.Rows.Add("org_inn", domain.Company.INN);
                        table.Rows.Add("org_kpp", domain.Company.KPP);
                        table.Rows.Add("org_orgn", domain.Company.ORGN);
                        table.Rows.Add("org_bik", domain.Company.BIK);
                        table.Rows.Add("org_bank", domain.Company.BankName);
                        table.Rows.Add("org_kor", domain.Company.BankAccount);
                        table.Rows.Add("org_account", domain.Company.OrgAccount);
                        table.Rows.Add("org_address", domain.Company.OrgAddress);
                        table.Rows.Add("org_address_index", domain.Company.OrgAddressIndex);
                        table.Rows.Add("org_ur_address", domain.Company.UrOrgAddress);
                        table.Rows.Add("org_ur_address_index", domain.Company.UrOrgAddressIndex);
                        table.Rows.Add("org_mails", domain.Company.Email);
                        table.Rows.Add("org_phones", domain.Company.Phone);

                        var org_face_short = "";
                        var fio = "ИП".Equals(domain.Company.OrgType) ? domain.Company.OrgName : domain.Company.FaceAcceptDocument;
                        if (!string.IsNullOrEmpty(fio))
                        {
                            var token = fio.Split(' ');
                            org_face_short = token.First() + " " + string.Join("", token.Skip(1).Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Substring(0, 1) + "."));
                        }
                        table.Rows.Add("org_face_short", org_face_short);

                        var pay = new System.Data.DataTable("r");
                        pay.Columns.Add("date");
                        pay.Columns.Add("sum");
                        pay.Columns.Add("desc");
                        ds.Tables.Add(pay);
                        foreach (var item in items)
                        {
                            pay.Rows.Add(
                                item.date.ToShortDateString(),
                                (item.sum_with_factor * -1).ToString("N2"),
                                item.transactions_number);
                        }

                        var total = items.Sum(p => p.sum_with_factor * -1);
                        table.Rows.Add("total", total.ToString("N2"));

                        var xmlDoc = new XmlDataDocument(ds);
                        var xt = new XslTransform();
                        var xtFile = Path.Combine(reportPath, "Resources", "report_checkout.xml");

                        using (var ms = new MemoryStream())
                        {
                            if (System.IO.File.Exists(xtFile))
                            {
                                xt.Load(xtFile);
                                xt.Transform(xmlDoc, null, ms);
                            }

                            var pdf = DocumentHelper.ConvetToPdf(ms.ToArray());
                            var aggregatorFile = new AggregatorFile()
                            {
                                Date = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)),
                                FileName = string.Format("{0}_report2_{1:yyyyMMdd}.pdf", domain.Login, date),
                                Group = StaticHelper.GroupCheckout,
                                Description = StaticHelper.GroupCheckout
                            };

                            var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, domain.Contract.Number);
                            if (!YandexDiskHelper.Folders.Exist(path))
                                YandexDiskHelper.Folders.Add(path);

                            var remoteFile = YandexDiskHelper.Folders.Combine(path, aggregatorFile.FileName);
                            YandexDiskHelper.Files.Upload(remoteFile, pdf);

                            var resource = YandexDiskHelper.Share.Publish(remoteFile);
                            if (resource != null && !string.IsNullOrEmpty(resource.public_key))
                            {
                                aggregatorFile.FileUrl = StaticHelper.GenerateDownloadUrl(resource.public_key);

                                AggregatorHelper.File.Add(domain.Agg, domain.Db, aggregatorFile);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                });

                Console.WriteLine();
            }
        }
    }
}
