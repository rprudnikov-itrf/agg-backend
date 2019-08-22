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
using CsvHelper;

namespace AG.Core.Task
{
    public static class СгенерироватьОтчетАгент
    {
        public static void Run(string reportPath, DateTime date)
        {
            Run(reportPath, date, null);
        }

        public static void Run(string reportPath, DateTime date, string dbId)
        {
            Console.WriteLine(date.ToShortDateString());

            var sdate = new DateTime(date.Year, date.Month, 1);
            var edate = sdate.AddMonths(1).AddDays(-1);

            var file = string.Format(@"E:\csv\report\report_balance_{0}_{1:00}.csv", date.Year, date.Month);
            if (!File.Exists(file))
                return;

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

            foreach (var agg in AggregatorHelper.Aggegator.List())
            {
                var reports = AggregatorHelper.Report.List(date, agg.Agg);
                if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
                    continue;

                var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
                Parallel.ForEach(reports.Acts, opt, report =>
                {
                    if (!string.IsNullOrWhiteSpace(dbId) && !string.Equals(report.Key, dbId, StringComparison.CurrentCultureIgnoreCase))
                        return;
                    
                    Console.Write(".");

                    try
                    {
                        if (report.Value == null || report.Value.hide)
                            return;

                        var domain = AggregatorHelper.Client.Get(agg.Agg, report.Key);
                        if (domain == null || domain.Contract == null || string.IsNullOrEmpty(domain.Contract.Number) || domain.Company == null)
                            return;

                        var key = domain.City + "_" + domain.Login;
                        if (!reportsCsv.ContainsKey(key))
                            return;

                        var reportCsv = reportsCsv[key];


                        var ds = new DataSet();
                        var table = new System.Data.DataTable("p");
                        table.Columns.Add("name");
                        table.Columns.Add("value");
                        ds.Tables.Add(table);

                        table.Rows.Add("act_number", date.ToString("MMyyyy"));
                        table.Rows.Add("act_date", date.AddMonths(1).AddDays(-1).ToString("dd MMMM yyyy"));
                        table.Rows.Add("act_period", date.ToString("MMMM yyyy"));

                        //1.Возмещаемые расходы, связанные с исполнением поручени
                        table.Rows.Add("act_complete_commission_yandex", (reportCsv.Удержана_комиссия_Яндекс + reportCsv.Покупка_смен).ToString("N2"));

                        //База для расчета Агентского вознаграждения составляет
                        table.Rows.Add("act_complete_cost", reportCsv.База.ToString("N2"));

                        //2.Вознаграждение Агента
                        table.Rows.Add("act_complete_commission_rostaxi", reportCsv.Удержана_комиссия_АТ.ToString("N2"));

                        //Перечислено по заявке на счет Принципала
                        table.Rows.Add("delete_balance", reportCsv.Перечислено_парку.ToString("N2"));

                        //Перечислено по заявке на счет Принципала
                        table.Rows.Add("add_balance", reportCsv.Пополнения_от_Принципала.ToString("N2"));
                        
                        var БН = reportCsv.Возвраты_перечислений_парку
                            + reportCsv.Возвраты_прочие
                            + reportCsv.БН_заказы
                            + reportCsv.Компенсации
                            + reportCsv.Чаевые
                            + reportCsv.Пополнение_от_QIWI
                            + reportCsv.Пополнения_от_Принципала
                            - reportCsv.Штрафы_Я
                            - reportCsv.Возвраты_Пользователям
                            - reportCsv.Ручные_возвраты_техподдержкой;

                        //Получено безналичных платежей от Пользователей в адрес Принципала (с учетом возвратов, компенсаций Сервиса Яндекс.Такси
                        table.Rows.Add("act_add_card_pay", Math.Max(БН, 0).ToString("N2"));

                        //Получено безналичных платежей от ООО «Яндекс.Такси» в пользу Принципала за оказание автотранспортных услуг по 
                        //продвижению Сервиса и иных услуг в соответствии с условиями требований работы Сервиса Яндекс.Такси
                        table.Rows.Add("add_bonus_pay", (reportCsv.Cубсидии + reportCsv.Купоны).ToString("N2"));

                        //Получено безналичных платежей от ООО «Яндекс.Такси» в пользу Принципала за оказание автотранспортных услуг по 
                        //перевозке Корпоративных Пользователей автомобильным транспортом
                        table.Rows.Add("act_add_corp_pay", reportCsv.Корпаративные_заказы.ToString("N2"));

                        //Итого задолженность Агента по расчетом перед Принципалом на конец месяца
                        //table.Rows.Add("act_tanker", (reportCsv.Заправки > 0 ? reportCsv.Долг_АТ : 0).ToString("N2"));
                        
                        table.Rows.Add("document_number", domain.Contract.Number);
                        table.Rows.Add("document_date", domain.Contract.Date.ToShortDateString());
                        table.Rows.Add("document_date_str", domain.Contract.Date.ToString("dd MMMM yyyy"));
                        table.Rows.Add("document_date_end_str", domain.Contract.Date.ToString("dd MMMM yyyy"));

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

                        var xmlDoc = new XmlDataDocument(ds);
                        var xt = new XslTransform();
                        var xtFile = Path.Combine(reportPath, "Resources", "report_agent.xml");

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
                            var aggregatorFile = new AggregatorFile()
                            {
                                Date = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)),
                                FileName = string.Format("{0}_act_{1:yyyyMMdd}.pdf", domain.Login, date),
                                Group = StaticHelper.GroupRepot,
                                Description = StaticHelper.GroupRepot
                            };

                            //if (!string.IsNullOrEmpty(dbId))
                            //{
                            //    File.WriteAllBytes(@"e:\ag\" + aggregatorFile.FileName, pdf);
                            //    return;
                            //}

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
