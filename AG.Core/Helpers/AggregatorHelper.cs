using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using RestSharp;
using AG.Core.Models;
using System.IO;
using System.Xml;

namespace AG.Core.Helpers
{
    public static class AggregatorHelper
    {
        private static RestClient _client;

        static AggregatorHelper()
        {
            _client = new RestClient("https://agg.taximeter.yandex.ru/api/");
            _client.AddDefaultHeader("apikey", ConfigurationManager.AppSettings["Apikey"]);
            _client.AddDefaultParameter("format", "json", ParameterType.QueryString);
        }

        public static class Client
        {
            public static AggregatorClient Get(string agg, string db)
            {
                var request = new RestRequest("clients/item");
                request.AddParameter("agg", agg);
                request.AddParameter("db", db);
                var response = _client.GetRetry<AggregatorClient>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }

            public static HashSet<string> Disabled()
            {
                var enable = List();
                var all = List(true);
                var disabled = all.Except(enable, new AggregatorClient());
                var result = new HashSet<string>(disabled.Select(p => p.Agg + ":" + p.Db));
                return result;
            }

            public static List<AggregatorClient> List(bool include_disabled = false)
            {
                return List(string.Empty, include_disabled);
            }
            public static List<AggregatorClient> List(string agg, bool include_disabled = false)
            {
                var request = new RestRequest("clients");
                if(!string.IsNullOrEmpty(agg))
                    request.AddParameter("agg", agg);

                if (include_disabled)
                    request.AddParameter("include_disabled", include_disabled);

                var response = _client.Get<List<AggregatorClient>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }
        }

        public static class Aggegator
        {
            public static AggregatorItem Get(string agg)
            {
                var request = new RestRequest("aggregators/item");
                request.AddParameter("agg", agg);
                var response = _client.GetRetry<AggregatorItem>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }

            public static List<AggregatorItem> List()
            {
                var request = new RestRequest("aggregators");
                var response = _client.Get<List<AggregatorItem>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }
        }

        public static class Report
        {
            public static List<AggregatorAct> Get(string agg, string db, params DateTime[] date)
            {
                var result = new List<AggregatorAct>();
                if (date == null || date.Count() == 0)
                    return result;

                foreach (var item in date)
                {
                    var items = Get(agg, db, item);
                    if (items != null)
                        result.Add(items);
                }

                return result;
            }

            public static AggregatorAct Get(string agg, string db, DateTime date)
            {
                var request = new RestRequest("report/item");
                request.AddParameter("agg", agg);
                request.AddParameter("db", db);
                request.AddParameter("date", date.ToString("yyyy-MM-dd"));
                var response = _client.GetRetry<AggregatorAct>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                        return Get(agg, db, date);

                    throw response.ErrorException;
                }

                return response.Data;
            }

            public static void Set(DateTime date, string db, AggregatorAct item)
            {
                var client = new RestClient(_client.BaseUrl);
                var request = new RestRequest("report/item");
                request.AddQueryParameter("apikey", ConfigurationManager.AppSettings["Apikey"]);
                request.AddQueryParameter("agg", item.agg);
                request.AddQueryParameter("date", date.ToString("yyyy-MM-dd"));
                request.AddQueryParameter("db", db);
                request.AddJsonBody(item);
                var response = client.PostRetryNotFound(request, 20);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                Console.WriteLine("\t" + response.StatusCode);
            }

            public static Dictionary<string, AggregatorAct> Set(string agg, DateTime date, string db, double sum)
            {
                var request = new RestRequest("report/set");
                request.AddParameter("agg", agg);
                request.AddParameter("date", date.ToString("yyyy-MM-dd"));
                request.AddParameter("db", db);
                request.AddParameter("sum", XmlConvert.ToString(sum));
                var response = _client.GetRetry<Dictionary<string, AggregatorAct>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }

            public static AggregatorReport List(DateTime date)
            {
                return List(date, string.Empty);
            }
            public static AggregatorReport List(DateTime date, string agg)
            {
                var request = new RestRequest("report");
                if(!string.IsNullOrEmpty(agg))
                    request.AddParameter("agg", agg);
                request.AddParameter("date", date.ToString("yyyy-MM-dd"));
                var response = _client.GetRetry<AggregatorReport>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                if (response.Data != null && response.Data.Acts != null)
                {
                    foreach (var item in response.Data.Acts)
                    {
                        item.Value.db = item.Key;
                        item.Value.agg = agg;
                    }
                }

                return response.Data;
            }
        }

        public static class File
        {
            public static AggregatorFileResponse Add(string agg, string db, AggregatorFile file)
            {
                if (file == null || string.IsNullOrEmpty(file.FileName))
                    return null;

                var id = Path.GetFileNameWithoutExtension(file.FileName).Md5();
                return Add(agg, db, id, file);
            }

            public static AggregatorFileResponse Add(string agg, string db, string id, AggregatorFile file)
            {
                var request = new RestRequest("files/add");
                request.AddParameter("agg", agg, ParameterType.QueryString);
                request.AddParameter("db", db, ParameterType.QueryString);
                request.AddParameter("id", id, ParameterType.QueryString);
                request.AddJsonBody(file);

                var response = _client.PostRetry<AggregatorFileResponse>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }

            public static bool Delete(string agg, string db, string id)
            {
                var request = new RestRequest("files/delete");
                request.AddParameter("agg", agg);
                request.AddParameter("db", db);
                request.AddParameter("id", id);

                var response = _client.Get(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    return false;

                return true;
            }

            public static Dictionary<string, AggregatorFile> List(string agg, string db)
            {
                var request = new RestRequest("files");
                request.AddParameter("agg", agg);
                request.AddParameter("db", db);

                var response = _client.GetRetry<Dictionary<string, AggregatorFile>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }
        }

        public static class Transaction
        {
            public static List<AggregatorTransaction> List(string agg, string db, string q, DateTime? sdate, DateTime? edate)
            {
                var result = new List<AggregatorTransaction>();

                for (var page = 0; ; page++)
                {
                    var request = new RestRequest("transactions");
                    request.AddParameter("agg", agg);
                    request.AddParameter("db", db);
                    request.AddParameter("page", page);

                    if (sdate.HasValue)
                        request.AddParameter("sdate", sdate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));

                    if (edate.HasValue)
                        request.AddParameter("edate", edate.Value.ToString("yyyy-MM-ddTHH:mm:ss"));

                    if (!string.IsNullOrEmpty(q))
                        request.AddParameter("q", q);

                    var response = _client.GetRetry<AggregatorTransactionResult>(request);
                    if (response.ResponseStatus != ResponseStatus.Completed)
                        throw response.ErrorException;

                    if (response.Data != null && response.Data.Items != null)
                        result.AddRange(response.Data.Items);

                    if (response.Data == null || Math.Min(response.Data.PageCount, 100) <= page + 1)
                        break;

                    if (response.Data.PageCount > 50)
                        Console.WriteLine("{0} = {1}", db, response.Data.PageCount);

                }

                return result;
            }
        }

        public static class Pays
        {
            public static List<AggregatorPay> List(string agg, string db, DateTime? sdate, DateTime? edate, bool full = false)
            {
                var result = new List<AggregatorPay>();

                for (var page = 0; ; page++)
                {
                    var request = new RestRequest("pays");
                    request.AddParameter("agg", agg);
                    request.AddParameter("db", db);
                    request.AddParameter("page", page);

                    if (sdate.HasValue)
                        request.AddParameter("sdate", (sdate.Value.Kind == DateTimeKind.Utc ? sdate.Value : TimeZoneInfo.ConvertTimeToUtc(sdate.Value, TimeZoneInfo.Local)).ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    if (edate.HasValue)
                        request.AddParameter("edate", (edate.Value.Kind == DateTimeKind.Utc ? edate.Value : TimeZoneInfo.ConvertTimeToUtc(edate.Value, TimeZoneInfo.Local)).ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    var response = _client.GetRetry<AggregatorPayResult>(request);
                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                            return List(agg, db, sdate, edate, full);

                        throw response.ErrorException;
                    }

                    if (response.Data != null && response.Data.Items != null)
                        result.AddRange(response.Data.Items);

                    if (!full)
                        break;

                    if (full && response.Data == null || Math.Min(response.Data.PageCount, 100) <= page + 1)
                        break;

                    //if (response.Data.PageCount > 50)
                    //    Console.WriteLine("{0} = {1}", db, response.Data.PageCount);

                }

                foreach (var item in result)
                    item.date = TimeZoneInfo.ConvertTimeToUtc(item.date, TimeZoneInfo.Local);

                return result;
            }

            public static List<AggregatorPay> Group(string agg, string db, DateTime? sdate, DateTime? edate)
            {
                var request = new RestRequest("pays/group/db");
                request.AddParameter("agg", agg);
                request.AddParameter("db", db);

                if (sdate.HasValue)
                    request.AddParameter("sdate", sdate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                if (edate.HasValue)
                    request.AddParameter("edate", edate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                //{"sum":191692.602359,"group":3,"factor":-1,"sum_with_factor":-191692.602359}
                var response = _client.GetRetry<List<AggregatorPay>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data ?? new List<AggregatorPay>();
            }
        }

        public static class Bill
        {
            public static Dictionary<string, AggregatorBill> Get(string agg)
            {
                var request = new RestRequest("bill");
                request.AddParameter("agg", agg);
                var response = _client.GetRetry<Dictionary<string, AggregatorBill>>(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;

                return response.Data;
            }

            public static void Set(string agg, string user, DateTime date, string number, double sum)
            {
                var request = new RestRequest("bill/set");
                request.AddParameter("agg", agg);
                request.AddParameter("user", user);
                request.AddParameter("date", date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"));
                request.AddParameter("number", number);
                request.AddParameter("sum", XmlConvert.ToString(sum));

                var response = _client.Get(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;
            }

            public static void Remove(string agg, string number)
            {
                var request = new RestRequest("bill/remove");
                request.AddParameter("agg", agg);
                request.AddParameter("number", number);

                var response = _client.Get(request);
                if (response.ResponseStatus != ResponseStatus.Completed)
                    throw response.ErrorException;
            }

            public static void Update(DateTime date)
            {
                Console.WriteLine(date.ToString("MMMM yyyy"));

                foreach (var agg in AggregatorHelper.Aggegator.List().OrderBy(p => p.City))
                {
                    Console.WriteLine(agg.City);
                    Update(agg.Agg, date);
                }
            }

            public static void Update(string agg, DateTime date)
            {
                try
                {
                    var sdate = Convert.ToDateTime("01." + date.ToString("MM.yyyy"));
                    var edate = sdate.AddMonths(1).AddMilliseconds(-1);

                    var sumBill = AggregatorHelper.Bill.Get(agg)
                        .Where(w => w.Value.date >= sdate && w.Value.date <= edate)
                        .Sum(p => p.Value.sum);

                    if (Math.Abs(sumBill) <= 0)
                    {
                        Console.WriteLine("\tСумма счета равна 0, расчет актов не возможен");
                        return;
                    }

                    var currentSumBill = sumBill;
                    var reports = AggregatorHelper.Report.List(sdate, agg);
                    var sumComissiom = reports.Acts.Sum(p => p.Value.complete_commission_yandex);

                    if (Math.Abs(sumComissiom) <= 0)
                    {
                        Console.WriteLine("\tСумма комиссий равна 0, расчет актов не возможен");
                        return;
                    }

                    var items = reports.Acts.Where(p => p.Value.complete_commission_yandex > 0).OrderBy(p => p.Key).ToList();
                    foreach (var item in items)
                    {
                        var cost = 0d;
                        if (item.Key == items.LastOrDefault().Key)
                            cost = currentSumBill;
                        else
                            cost = Math.Round(item.Value.complete_commission_yandex / sumComissiom * sumBill, 2);

                        currentSumBill -= cost;

                        AggregatorHelper.Report.Set(agg, date, item.Key, cost);
                    }

                    //сверка
                    var total = AggregatorHelper.Report.List(sdate, agg).Acts.Sum(p => p.Value.complete_commission_yandex_bill);
                    if (Math.Abs(total - sumBill) < 1)
                        Console.WriteLine("\tOK");
                    else
                        Console.WriteLine("\t{0:N2} != {1:N2}", total, sumBill);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\t" + ex.Message);
                }
            }
        }
    }
}