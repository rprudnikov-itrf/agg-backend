using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using AG.Core.Helpers;
using System.Data;
using AG.Core.Models;
using AG.Core.Models.YandexDisk;
using System.Threading.Tasks;

namespace AG.Core.Task
{
    public static class СгенерироватьРасторжение
    {
        public static void Run(string reportPath)
        {
            var domains = AggregatorHelper.Client.List()
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number) && p.Contract.Signed && p.Contract.Date < new DateTime(2016, 12, 1))
                .OrderBy(p => p.Contract.Number.TryParseInt32())
                .ToArray();

            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
            Parallel.ForEach(domains, opt, domain =>
            {
                try
                {
                    //if (!"682a7cf39169425aab48899880767b99".Equals(domain.Db))
                    //    return;

                    var files = Run(domain, reportPath);
                    if (files == null)
                    {
                        Console.WriteLine("{0}\tNot Found", domain.Contract.Number);
                        return;
                    }

                    Console.WriteLine("{0}\tOK - {1}", domain.Contract.Number, files.public_key);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        public static Resource Run(string agg, string db, string reportPath, bool test = false)
        {
            var client = AggregatorHelper.Client.Get(agg, db);
            return Run(client, reportPath, test);
        }

        public static Resource Run(AggregatorClient client, string reportPath, bool test = false)
        {
            if (client == null || client.Contract == null || string.IsNullOrEmpty(client.Contract.Number) || client.Company == null)
                return null;

            var filename = string.Format("dissolution_{0}_{1:yyyyMMdd}.pdf", client.Contract.Number, client.Contract.Date);
            var remotePath = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, client.Contract.Number);
            var remoteFile = YandexDiskHelper.Folders.Combine(remotePath, filename);
            if (YandexDiskHelper.Folders.Exist(remoteFile) && !test)
            {
                var resource = YandexDiskHelper.Folders.Resources(remoteFile);
                if (resource != null && !string.IsNullOrEmpty(resource.public_key))
                    return resource;
            }

            var ds = new DataSet();
            var table = new System.Data.DataTable("p");
            table.Columns.Add("name");
            table.Columns.Add("value");
            ds.Tables.Add(table);

            table.Rows.Add("document_number", string.Format("{0}-{1:yyyy}", client.Contract.Number, client.Contract.Date));
            table.Rows.Add("document_date", client.Contract.Date.ToShortDateString());
            table.Rows.Add("document_date_str", client.Contract.Date.ToString("dd MMMM yyyy"));
            table.Rows.Add("date_str", DateTime.Today.ToString("dd MMMM yyyy"));
            table.Rows.Add("balance", client.Balance.ToString("N2"));
            table.Rows.Add("db", client.Login);

            table.Rows.Add("org_type", client.Company.OrgType);
            table.Rows.Add("org_name", client.Company.OrgName);
            table.Rows.Add("org_base", client.Company.BaseDocument);
            table.Rows.Add("org_face", client.Company.FaceAcceptDocument);
            table.Rows.Add("org_face_post", client.Company.Post);
            table.Rows.Add("org_inn", client.Company.INN);
            table.Rows.Add("org_kpp", client.Company.KPP);
            table.Rows.Add("org_orgn", client.Company.ORGN);
            table.Rows.Add("org_bik", client.Company.BIK);
            table.Rows.Add("org_bank", client.Company.BankName);
            table.Rows.Add("org_kor", client.Company.BankAccount);
            table.Rows.Add("org_account", client.Company.OrgAccount);
            table.Rows.Add("org_address", client.Company.OrgAddress);
            table.Rows.Add("org_address_index", client.Company.OrgAddressIndex);
            table.Rows.Add("org_ur_address", client.Company.UrOrgAddress);
            table.Rows.Add("org_ur_address_index", client.Company.UrOrgAddressIndex);
            table.Rows.Add("org_mails", client.Company.Email);
            table.Rows.Add("org_phones", client.Company.Phone);

            var org_face_short = "";
            var fio = (("ИП".Equals(client.Company.OrgType) ? client.Company.OrgName : client.Company.FaceAcceptDocument) ?? "").Trim();
            if (!string.IsNullOrEmpty(fio))
            {
                var token = fio.Split(' ');
                org_face_short = token.First() + " " + string.Join("", token.Skip(1).Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Substring(0, 1) + "."));
            }
            table.Rows.Add("org_face_short", org_face_short);

            var xmlDoc = new XmlDataDocument(ds);
            var xt = new XslTransform();
            var xtFile = Path.Combine(reportPath, "Resources", "contract_dissolution.xml");

            using (var file = new MemoryStream())
            {
                if (System.IO.File.Exists(xtFile))
                {
                    xt.Load(xtFile);
                    xt.Transform(xmlDoc, null, file);
                }

                if (test)
                {
                    File.WriteAllBytes("r:\\contract_dissolution.doc", file.ToArray());
                    return null;
                }
                else
                {
                    var pdf = DocumentHelper.ConvetToPdf(file.ToArray());
                    if (!YandexDiskHelper.Folders.Exist(remotePath))
                        YandexDiskHelper.Folders.Add(remotePath);

                    YandexDiskHelper.Files.Upload(remoteFile, pdf);
                    var resource = YandexDiskHelper.Share.Publish(remoteFile);
                    if (resource == null || string.IsNullOrEmpty(resource.public_key))
                        return null;

                    var id = Path.GetFileNameWithoutExtension(filename).Md5();
                    AggregatorHelper.File.Add(client.Agg, client.Db, id, new AggregatorFile()
                    {
                        Group = "Договор",
                        FileName = filename,
                        Date = client.Contract.Date,
                        Description = string.Format("Соглашение о расторжении договора №{0}-{1:yyyy}", client.Contract.Number, client.Contract.Date),
                        FileUrl = StaticHelper.GenerateDownloadUrl(resource.public_key)
                    });

                    return resource;
                }
            }
        }
    }
}
