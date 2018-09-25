using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.Core.Helpers;
using System.Threading;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Net.Mail;
using RestSharp;

namespace AG.Core.Task
{
    public static class РассылкаОтчетаАгента
    {
        public static void Run(DateTime date)
        {
            Console.WriteLine("Рассылка отчета агента {0:MMMM yyyy}", date);

            var login = System.Configuration.ConfigurationManager.AppSettings["MailLogin"];
            var password = System.Configuration.ConfigurationManager.AppSettings["MailPassword"];
            var server = System.Configuration.ConfigurationManager.AppSettings["MailSmtp"];
            var port = System.Configuration.ConfigurationManager.AppSettings["MailPort"];

            var clients = AggregatorHelper.Client.List();
            var domains = clients
                .Where(p => p.Contract != null && !string.IsNullOrEmpty(p.Contract.Number) && p.Contract.Signed
                         && p.Company != null && !string.IsNullOrWhiteSpace(p.Company.Email))
                .OrderBy(p => p.Login ?? "")
                .ToArray();

            var i = 0;
            foreach (var domain in domains)
            {
                try
                {
                    var files = AggregatorHelper.File.List(domain.Agg, domain.Db);
                    if (files == null || files.Count == 0)
                        continue;

                    var file = files.FirstOrDefault(p => p.Value.Date.Year == date.Year && p.Value.Date.Month == date.Month && string.Equals(StaticHelper.GroupRepot, p.Value.Group));
                    if (file.Value == null || string.IsNullOrWhiteSpace(file.Value.FileUrl))
                        continue;

                    var restClient = new RestClient(file.Value.FileUrl);
                    var fileContent = restClient.DownloadData(new RestRequest());
                    if (fileContent == null || fileContent.Length == 0)
                        continue;

                    Console.WriteLine("{0} - {1}", domain.Login, domain.City);

                    using (var smtpClient = new SmtpClient(server, port.TryParseInt32()))
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(login, password);
                        smtpClient.EnableSsl = true;

                        var msg = new System.Net.Mail.MailMessage()
                        {
                            IsBodyHtml = true,
                            From = new MailAddress(login),
                            BodyEncoding = Encoding.UTF8,
                            SubjectEncoding = Encoding.UTF8,
                        };
                        
                        msg.Subject = string.Format("Отчет об исполнении агентского поручения по сервису Яндекс.Такси за {0:MMMM yyyy}", date);
                        msg.Body = string.Format(@"
Добрый день!<br />
В Ваш адрес отправлен отчет об исполнении агентского поручения по сервису Яндекс.Такси по договору {0} за {1:MMMM yyyy} года в электронном виде (отчет находится в приложении к данному письму).<br /><br />

Так же отчетные документы размещены у вас в диспетчерской в разделе Личный кабинет. <br /><br />

Благодарим за сотрудничество. <br /><br /><br /><br />


-- <br />
С уважением, <br />
команда Агрегатор такси <br />
support@rostaxi.info <br />
Tel.: +7 (499) 391-72-86<br /><br />", domain.Contract.Number, date);

                        var fileStream = new MemoryStream(fileContent);
                        var attachment = new Attachment(fileStream, file.Value.FileName);
                        attachment.NameEncoding = Encoding.UTF8;
                        msg.Attachments.Add(attachment);

                        msg.To.Add(domain.Company.Email);
                        smtpClient.Send(msg);
                    }

                    Thread.Sleep(++i % 50 == 0 ? 600000 : 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
