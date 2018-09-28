using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AG.Core.Helpers
{
    public static class StaticHelper
    {
        public const string ClientsFolder = "/Рос.Такси/Контрагенты/";
        public const string ClientsArhivFolder = "/Рос.Такси/Контрагенты/~arhiv/";
        public const string ReportsFolder = "/Рос.Такси/Сделка/Отчет агента/";

        public const string GroupBill = "Счета-фактуры";
        public const string GroupRepot = "Отчет агента";
        public const string GroupCheckout = "Отчет о перечеслении";

        public static string GenerateDownloadUrl(string public_key)
        {
            if (string.IsNullOrEmpty(public_key))
                return string.Empty;
                     
            return "http://doc.rostaxi.info/download?file=" + HttpUtility.UrlEncode(public_key);
        }
    }
}
