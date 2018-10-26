using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AG.Core.Helpers;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using AG.Core.Models;

namespace AG.Core.Task
{
    public static class ЗагрузитьОтчетПоКомиссии
    {
        private static List<string> _log = new List<string>();

        public static void Run(string reportPath)
        {
            var total = 0d;
            for (var i = new DateTime(2018, 8, 1); i <= new DateTime(2018, 9, 1); i = i.AddMonths(1))
            {
                total += Run(reportPath, i);
            }

            Console.WriteLine(total);
        }

        public static double Run(string reportPath, DateTime date)
        {
            var culture = CultureInfo.GetCultureInfo("ru");
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
            var lockobj = new Object();
            var comission = 0.50d;
            var total = 0d;
            var client = AggregatorHelper.Client.List();

            var ds = new DataSet();
            var table = new System.Data.DataTable("p");
            table.Columns.Add("name");
            table.Columns.Add("value");
            ds.Tables.Add(table);

            var item = new System.Data.DataTable("item");
            item.Columns.Add("login");
            item.Columns.Add("inn");
            item.Columns.Add("total");
            item.Columns.Add("comission");
            ds.Tables.Add(item);

            Parallel.ForEach(AggregatorHelper.Aggegator.List(), opt, agg =>
            {
                try
                {
                    var reports = AggregatorHelper.Report.List(date, agg.Agg);
                    if (reports == null || reports.Acts == null || reports.Acts.Count == 0)
                        return;

                    lock (lockobj)
                    {
                        foreach (var i in reports.Acts
                            .Where(p => clients.Contains(p.Value.db_name))
                            .OrderBy(p => p.Value.db_name))
                        {
                            var _total = i.Value.complete_commission_rostaxi + i.Value.cancel_commission_rostaxi;
                            if (_total == 0)
                                continue;

                            total += _total;

                            if (string.IsNullOrWhiteSpace(i.Value.inn))
                            {
                                var c = client.FirstOrDefault(p => string.Equals(p.Db, i.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (c != null && c.Company != null)
                                    i.Value.inn = c.Company.INN; 
                            }

                            item.Rows.Add(
                                i.Value.db_name,
                                i.Value.inn,
                                _total.ToString("N2"),
                                (_total * comission).ToString("N2")
                                );
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            table.Rows.Add("number", date.ToString("yyyyMM"));
            table.Rows.Add("date", date.ToString("MMMM yyyy"));
            table.Rows.Add("day", new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)).ToString("dd MMMM yyyy"));
            table.Rows.Add("total", total.ToString("N2"));
            table.Rows.Add("comission", (total * comission).ToString("N2"));

            var xmlDoc = new XmlDataDocument(ds);
            var xt = new XslTransform();
            var xtFile = Path.Combine(reportPath, "Resources", "report_agent_taxi.xml");

            using (var ms = new MemoryStream())
            {
                if (System.IO.File.Exists(xtFile))
                {
                    xt.Load(xtFile);
                    xt.Transform(xmlDoc, null, ms);
                }

                var buffer = ms.ToArray();
                if (buffer.Length == 0)
                    return 0;

                var pdf = DocumentHelper.ConvetToPdf(buffer);

                if (!YandexDiskHelper.Folders.Exist(StaticHelper.ReportsFolder))
                    YandexDiskHelper.Folders.Add(StaticHelper.ReportsFolder);

                var remoteFile = YandexDiskHelper.Folders.Combine(StaticHelper.ReportsFolder, string.Format("report_{0:yyyyMM}.pdf", date));
                YandexDiskHelper.Files.Upload(remoteFile, pdf);
            }

            var log = string.Format("{0:MM.yyyy}\t{1:N2}", date, total * comission);
            Console.WriteLine(log);
            return total * comission;
        }

        public static HashSet<string> clients = new HashSet<string>()
        {
            "1331_регион",
            "5элемент",
            "7000000_спб",
            "746",
            "7724",
            "777-мск",
            "929_спб",
            "aig-spb",
            "ami",
            "art-taxi",
            "auto-v-arendu",
            "battaxi",
            "big_sity",
            "bigboss",
            "city_cabman",
            "clubspb_спб",
            "dav",
            "dias",
            "elegance",
            "familyexpress_спб",
            "funtaxi",
            "g12_спб",
            "garryx",
            "geo-taxi",
            "gorodtaxi",
            "gush",
            "home_taxi",
            "in_time_спб",
            "isfara",
            "lepidus_мск",
            "lsl-taxi",
            "midnight",
            "mishline",
            "mobile_cab",
            "mopstaxi",
            "mostaxi24",
            "mostaxi24_com",
            "multicab",
            "passazhir-taxi",
            "platon",
            "profit-taxi",
            "provide_taxi",
            "rodion-taxi",
            "runtransport",
            "samson",
            "sl_taxi",
            "sl-taxi",
            "solo_taxi",
            "soul",
            "status_best",
            "sunny_taxi",
            "sv_taxi",
            "tamat24",
            "taxi_on_time",
            "taxi181",
            "taxi2007",
            "taxi-club",
            "taxilana",
            "taxi-lotta",
            "taxi-online24",
            "taxireversi",
            "urban",
            "vinni-puh",
            "vit-taxi",
            "vojage",
            "vtaxi",
            "yellow_time",
            "yunyi_flight_taxi",
            "а1-такси",
            "абл_такси",
            "авантаж",
            "авто_глобал",
            "автогруз_спб",
            "автомакс",
            "авто-миг",
            "автосоло_спб",
            "автостолица",
            "автотехнополис_спб",
            "автофит",
            "авто-шор",
            "агат_сб",
            "адамас_мск",
            "алгоритм",
            "алиша",
            "альбатрос_такси",
            "альфа-т_спб",
            "андрей",
            "аполлон123_мск",
            "апрель",
            "арбат_такси",
            "арион-ри",
            "арт_такси",
            "атланта",
            "аура",
            "ашто_спб",
            "без_оленей",
            "белый_лебедь",
            "бизнесавто",
            "бизнессервис",
            "блеск_авто",
            "бонус_такси",
            "борт_такси",
            "вам_такси",
            "виал",
            "виктория_мск",
            "восход_мск",
            "время_комфорта",
            "всетакси_спб",
            "встреча_мск",
            "вулкан",
            "галактика",
            "геккон_мск",
            "глобус_такси",
            "город_мск",
            "городское_троицк",
            "гп_такси",
            "гранд_авто",
            "гуляй_город",
            "дарьюшка",
            "димиан",
            "домашний_уют",
            "дрифт_такси",
            "изумруд_такси",
            "империя_мск",
            "империяавтотранс",
            "калипсо",
            "кв_мск",
            "квн-такси",
            "клаксон",
            "комфортное",
            "комфортное_такси",
            "конек-горбунок",
            "крыло",
            "ладья_мск",
            "леогранд-такси",
            "лидер",
            "луч_мск",
            "маршрут",
            "маяк_мск",
            "мдс",
            "метеор_спб",
            "микс_спб",
            "микс2_спб",
            "миледи_мск",
            "минутка",
            "мираж",
            "мл_такси",
            "молния_vip",
            "монтана",
            "мосинтакси",
            "Мостакси",
            "м-сити_групп",
            "м-такси_спб",
            "мэр",
            "навигатор1_мск",
            "наринка",
            "настроение_авто",
            "нв-групп",
            "недорого",
            "ника_такси",
            "никос",
            "новое",
            "ньюмоскоутакси",
            "орион_мск",
            "пандатакси",
            "паритет_спб",
            "пегас_мск",
            "пегас_спб",
            "персона_спб",
            "персональный",
            "пилигрим_такси",
            "пипелатц",
            "планета_мск",
            "плутон",
            "плюс_7_спб",
            "прокат-такси",
            "промтекс",
            "протон",
            "пума",
            "пушкинъ",
            "пять_плюс",
            "реал",
            "рестар",
            "ритм_города",
            "рмtaxi",
            "роман_мск",
            "ростакси_спб",
            "рускеб",
            "русскаятройка",
            "русский_путь",
            "русь2",
            "рядом_спб",
            "сальса",
            "санкт-петербург_спб",
            "сент-элена",
            "сила_такси",
            "сириус_мск",
            "сител_авто",
            "сити_драйв",
            "сити_лайн",
            "ситиас",
            "словянское",
            "слон",
            "смайлстурс",
            "смирноф_такси",
            "спектр_спб",
            "спутник_спб",
            "старое_доброе_такси",
            "старый_друг_спб",
            "стигл-м",
            "стиль_спб",
            "стк_антарес",
            "столица-такси",
            "столичное_такси",
            "стриж_спб",
            "сэртакси",
            "такса_мск",
            "такси_да",
            "такси_миг",
            "такси_москва",
            "такси_москвы",
            "такси_рф",
            "такси_удача",
            "такси_яна",
            "такси-1745",
            "такси33",
            "такси740",
            "такси888",
            "таксимаксим",
            "таксист",
            "таксом",
            "таксопарк_17",
            "тачки",
            "терра_21век",
            "тк_аргамак",
            "тк_безназвания",
            "тк_вега_спб",
            "тк_вояж",
            "тк_данияр",
            "транс-м",
            "трансмира",
            "трансфертаксии",
            "трансфертаксииру",
            "триумф-такси",
            "ударник",
            "универсальное",
            "форвард",
            "фортуна",
            "хэппи",
            "чекерс",
            "шпак-такси",
            "шпиль",
            "эверест_мск",
            "эдс",
            "эй-би-си_спб",
            "эко_такси_мск",
            "экспресс_мск",
            "экспресс_спб",
            "экстра-люкс",
            "эмир",
            "это_я_такси_спб"
        };
    }
}
