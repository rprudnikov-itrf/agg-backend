using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Xml.Linq;
using AG.Core.Helpers;
using AG.Core.Models;
using System.Configuration;
using System.IO;

namespace AG.Core.Task
{
    public static class СгенерироватьСчетаФактуры
    {
        private const string НаименованиеСокращенное = "ООО \"АГРЕГАТОР ТАКСИ\"";
        private const string НаименованиеПолное = "Общество с ограниченной ответственностью \"АГРЕГАТОР ТАКСИ\"";
        private const string ИНН = "7735144097";
        private const string КПП = "773501001";

        public static void Run(DateTime date, string aggId = null)
        {
            Console.WriteLine(date.ToString("MMMM yyyy"));

            var syncFolder = Path.Combine(ConfigurationManager.AppSettings["SyncFolder"], date.ToString("yyyyMM"));
            if (!Directory.Exists(syncFolder))
                Directory.CreateDirectory(syncFolder);

            foreach (var item in Directory.GetFiles(syncFolder))
                File.Delete(item);

            var agg = AggregatorHelper.Aggegator.List();
            foreach (var item in agg.OrderBy(p => p.City))
	        {
                if (!string.IsNullOrEmpty(aggId) && !string.Equals(item.Agg, aggId, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                Run(item, date);
	        }
        }

        private static void Run(AggregatorItem agg, DateTime date)
        {
            var syncFolder = Path.Combine(ConfigurationManager.AppSettings["SyncFolder"], date.ToString("yyyyMM"));
            if (!Directory.Exists(syncFolder))
                Directory.CreateDirectory(syncFolder);

            var sdate = new DateTime(date.Year, date.Month, 1);
            var edate = sdate.AddMonths(1).AddMilliseconds(-1);
            var i = 0;

            var documents = new ConcurrentBag<XElement>();
            var items = AggregatorHelper.Report.List(sdate, agg.Agg).Acts.Where(p => !p.Value.hide && p.Value.complete_cost > 0).OrderBy(p => p.Value.db_name);

            Console.WriteLine("{0} = {1} = {2:N2}", agg.City, items.Count(), items.Sum(p => p.Value.complete_commission_yandex_bill));

            foreach (var item in items)
	        {
                var client = AggregatorHelper.Client.Get(agg.Agg, item.Key);
                if (client == null) continue;

                var company =  client.Company;
                if (company == null) continue;

                var contract = client.Contract;
                if (contract == null) continue;

                if (string.IsNullOrWhiteSpace(company.INN))
                    Console.WriteLine("\t{0} - empty INN", item.Value.db_name);

                i++;

                documents.Add(new XElement("Документ.ПоступлениеТоваровУслуг",
                    new XElement("КлючевыеСвойства",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "1" + sdate.ToString("MMyyyy")),
                        new XElement("Дата", edate),
                        new XElement("Номер", i + "-" + sdate.ToString("MM.yyyy")),
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо"))),
                    new XElement("Ответственный",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "2" + sdate.ToString("MMyyyy")),
                        new XElement("Наименование", "&lt;Не указан&gt;")),
                    new XElement("ВидОперации", "ПокупкаКомиссия"),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "1" + sdate.ToString("MMyyyy")),
                        new XElement("Код", 643)),
                    new XElement("СуммаДокумента", item.Value.complete_commission_yandex_bill),
                    new XElement("Контрагент",
                        new XElement("Ссылка", "123"),
                        new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                        new XElement("ИНН", "7736207543"),
                        new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                    new XElement("ДанныеВзаиморасчетов",
                        new XElement("Договор",
                            new XElement("Ссылка", "123"),
                            new XElement("Номер", "1"),
                            new XElement("Дата", "01.09.2015"),
                            new XElement("ВидДоговора", "СПоставщиком"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                                new XElement("ИНН", "7736207543"),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false)),
                        new XElement("ВалютаВзаиморасчетов",
                            new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                            new XElement("Код", 643)),
                        new XElement("КурсВзаиморасчетов", 1),
                        new XElement("КратностьВзаиморасчетов", 1),
                        new XElement("РасчетыВУсловныхЕдиницах", false)),
                    new XElement("НДСНеВыделять", true),
                    new XElement("СуммаВключаетНДС", true),
                    new XElement("АгентскиеУслуги",
                        new XElement("Строка",
                            new XElement("ДанныеНоменклатуры",
                                new XElement("Номенклатура",
                                    new XElement("Ссылка", item.Key.Remove(1, 5) + "3" + sdate.ToString("MMyyyy")),
                                    new XElement("НаименованиеПолное", "Информационные услуги Яндекс"),
                                    new XElement("КодВПрограмме", "00-00000002"))),
                            new XElement("Содержание", "Информационные услуги Яндекс"),
                            new XElement("Количество", "1"),
                            new XElement("Сумма", item.Value.complete_commission_yandex_bill),
                            new XElement("Цена", item.Value.complete_commission_yandex_bill),
                            new XElement("суммаНДС", (item.Value.complete_commission_yandex_bill * 20 / 120)),
                            new XElement("СтавкаНДС", "НДС20"),
                            new XElement("Контрагент",
                                new XElement("Ссылка", item.Key),
                                new XElement("НаименованиеСокращенное", company.OrgName),
                                new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                new XElement("ИНН",
                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                        "")),
                                string.IsNullOrWhiteSpace(company.KPP)
                                    ? null
                                    : new XElement("КПП",
                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
                                new XElement("ЮридическоеФизическоеЛицо",
                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                            new XElement("Договор",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Дата", contract.Date),
                                new XElement("Номер", contract.Number),
                                new XElement("ВидДоговора", "СКомитентомНаЗакупку"),
                                new XElement("Организация",
                                    new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                    new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                    new XElement("НаименованиеПолное", НаименованиеПолное),
                                    new XElement("ИНН", ИНН),
                                    new XElement("КПП", КПП),
                                    new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                                new XElement("Контрагент",
                                    new XElement("Ссылка", item.Key),
                                    new XElement("НаименованиеСокращенное", company.OrgName),
                                    new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                    new XElement("ИНН",
                                        System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                            "")),
                                    string.IsNullOrWhiteSpace(company.KPP)
                                        ? null
                                        : new XElement("КПП",
                                            System.Text.RegularExpressions.Regex
                                                .Replace(company.KPP, "[^0-9]", "")),
                                    new XElement("ЮридическоеФизическоеЛицо",
                                        company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                                new XElement("ВалютаВзаиморасчетов",
                                    new XElement("Ссылка", "123"),
                                    new XElement("Код", 643)),
                                new XElement("РасчетыВУсловныхЕдиницах", false),
                                new XElement("СчетРасчетов",
                                    new XElement("Ссылка", "123"),
                                    new XElement("Код", "76 .09. ")))
                        ))));

                documents.Add(new XElement("Документ.ОтчетКомитентуОРозничныхПродажах",
                    new XElement("КлючевыеСвойства",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "5" + sdate.ToString("MMyyyy")),
                        new XElement("Дата", edate),
                        new XElement("Номер", i + "-" + sdate.ToString("MM.yyyy")),
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо"))),
                    new XElement("ВидОперации", "ОтчетОПродажах"),
                    new XElement("СпособРасчетаКомиссионногоВознаграждения", "ПроцентОтСуммыПродажи"),
                    new XElement("Контрагент",
                        new XElement("Ссылка", item.Key),
                        new XElement("НаименованиеСокращенное", company.OrgName),
                        new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                        new XElement("ИНН",
                            System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]", "")),
                        string.IsNullOrWhiteSpace(company.KPP)
                            ? null
                            : new XElement("КПП",
                                System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
                        new XElement("ЮридическоеФизическоеЛицо",
                            company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                    new XElement("ДанныеВзаиморасчетов",
                        new XElement("Договор",
                            new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                            new XElement("Дата", contract.Date),
                            new XElement("Номер", contract.Number),
                            new XElement("ВидДоговора", "СКомитентом"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", item.Key),
                                new XElement("НаименованиеСокращенное", company.OrgName),
                                new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                new XElement("ИНН",
                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                        "")),
                                string.IsNullOrWhiteSpace(company.KPP)
                                    ? null
                                    : new XElement("КПП",
                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
                                new XElement("ЮридическоеФизическоеЛицо",
                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false))),
                    new XElement("ПроцентКомиссионногоВознаграждения", 2),
                    new XElement("СтавкаНДСВознаграждения", "БезНДС"),
                    new XElement("УдержатьВознаграждение", true),
                    new XElement("УслугаПоВознаграждению",
                        new XElement("Ссылка", item.Key.Remove(1, 3) + "444"),
                        new XElement("НаименованиеПолное", "Агентские услуги"),
                        new XElement("КодВПрограмме", "00-00000002")),
                    new XElement("СчетУчетаНДСПоРеализации", "90.03"),
                    new XElement("СчетДоходов", "90.01.1"),
                    new XElement("Ответственный",
                        new XElement("Ссылка", "123"),
                        new XElement("КодВПрограмме", "&amp;lt;Не указан&amp;gt;")),
                    new XElement("СуммаДокумента", item.Value.complete_cost),
                    new XElement("СуммаВознаграждения", item.Value.complete_commission_rostaxi),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", "123"),
                        new XElement("Код", 643)),
                    new XElement("КурсВзаиморасчетов", "1"),
                    new XElement("КратностьВзаиморасчетов", "1"),
                    new XElement("СуммаВключаетНДС", true),
                    new XElement("Товары",
                        new XElement("Строка",
                            new XElement("Номенклатура",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "Перевозка пассажиров"),
                                new XElement("КодВПрограмме", "00-00000002")),
                            new XElement("Количество", 1),
                            new XElement("Цена", item.Value.complete_cost),
                            new XElement("Сумма", item.Value.complete_cost),
                            new XElement("СтавкаНДС", "БезНДС"),
                            new XElement("СуммаНДС", 0),
                            new XElement("СуммаВознаграждения", item.Value.complete_commission_rostaxi),
                            new XElement("СуммаНДСВознаграждения", 0),
                            new XElement("Покупатель",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеСокращенное", "Физическое лицо"),
                                new XElement("НаименованиеПолное", "Физическое лицо"),
                                new XElement("ИНН", "1111111111"),
                                new XElement("КПП", "111111111"),
                                new XElement("ЮридическоеФизическоеЛицо", "ФизическоеЛицо")),
                            new XElement("ДатаРеализации", edate),
                            new XElement("Содержание", "Перевозка пассажиров"))),
                    new XElement("ДенежныеСредства",
                        new XElement("Строка",
                            new XElement("ВидОтчетаПоПлатежам", "Оплата"),
                            new XElement("Сумма", item.Value.complete_commission_rostaxi),
                            new XElement("СтавкаНДС", "БезНДС"),
                            new XElement("СуммаНДС", 0),
                            new XElement("ДатаСобытия", edate),
                            new XElement("Покупатель",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеСокращенное", "Физическое лицо"),
                                new XElement("НаименованиеПолное", "Физическое лицо"),
                                new XElement("ИНН", "1111111111"),
                                new XElement("КПП", "111111111"),
                                new XElement("ЮридическоеФизическоеЛицо", "ФизическоеЛицо"))))
                ));

                // платежи по карте Яндекса
                documents.Add(new XElement("Документ.ОплатаПлатежнойКартой",
                    new XElement("КлючевыеСвойства",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                        new XElement("Дата", edate),
                        new XElement("Номер", i + "-" + sdate.ToString("MM.yyyy")),
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо"))),
                    new XElement("Ответственный",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                        new XElement("Наименование", "&lt;Не указан&gt;")),
                    new XElement("ВидОперации", "ОплатаПокупателя"),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "1" + sdate.ToString("MMyyyy")),
                        new XElement("Код", 643)),
                    new XElement("СуммаДокумента", item.Value.card_pay),
                    new XElement("Контрагент",
                        new XElement("Ссылка", "123"),
                        new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                        new XElement("ИНН", "7736207543"),
                        new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                    new XElement("ДанныеВзаиморасчетов",
                        new XElement("Договор",
                            new XElement("Ссылка", "123"),
                            new XElement("Номер", "Эквайринг"),
                            new XElement("Дата", "01.09.2015"),
                            new XElement("ВидДоговора", "Прочее"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                                new XElement("ИНН", "7736207543"),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false)),
                        new XElement("ВалютаВзаиморасчетов",
                            new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                            new XElement("Код", 643)),
                        new XElement("КурсВзаиморасчетов", 1),
                        new XElement("КратностьВзаиморасчетов", 1),
                        new XElement("РасчетыВУсловныхЕдиницах", false)),
                    new XElement("ВидОплаты",
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                        new XElement("ТипОплаты", "ПлатежнаяКарта"),
                        new XElement("Контрагент",
                            new XElement("Ссылка", "123"),
                            new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                            new XElement("ИНН", "7736207543"),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                        new XElement("ДоговорКонтрагента",
                            new XElement("Ссылка", item.Key.Remove(1, 5) + "5" + sdate.ToString("MMyyyy")),
                            new XElement("Дата", edate),
                            new XElement("Номер", "Эквайринг"),
                            new XElement("ВидДоговора", "Прочее"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "ООО \"ЯНДЕКС\""),
                                new XElement("ИНН", "7736207543"),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false))),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                        new XElement("Код", 643)),
                    new XElement("РасшифровкаПлатежа",
                        new XElement("Строка",
                            new XElement("СуммаДокумента", item.Value.card_pay),
                            new XElement("КурсВзаиморасчетов", 1),
                            new XElement("КратностьВзаиморасчетов", 1),
                            new XElement("СпособПогашенияЗадолженности", "Автоматически"),
                            new XElement("СтавкаНДС", "БезНДС"),
                            new XElement("СчетУчетаРасчетовСКонтрагентом", "76.09"),
                            new XElement("СчетУчетаРасчетовПоАвансам", "76.09"),
                            new XElement("Контрагент",
                                new XElement("Ссылка", item.Key),
                                new XElement("НаименованиеСокращенное", company.OrgName),
                                new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                new XElement("ИНН",
                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                        "")),
                                string.IsNullOrWhiteSpace(company.KPP)
                                    ? null
                                    : new XElement("КПП",
                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
                                new XElement("ЮридическоеФизическоеЛицо",
                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                            new XElement("ДоговорКонтрагента",
                                new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                                new XElement("Дата", edate),
                                new XElement("Номер", contract.Number),
                                new XElement("ВидДоговора", "СКомитентом"),
                                new XElement("Организация",
                                    new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                    new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                    new XElement("НаименованиеПолное", НаименованиеПолное),
                                    new XElement("ИНН", ИНН),
                                    new XElement("КПП", КПП),
                                    new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                                new XElement("Контрагент",
                                    new XElement("Ссылка", item.Key),
                                    new XElement("НаименованиеСокращенное", company.OrgName),
                                    new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                    new XElement("ИНН",
                                        System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                            "")),
                                    string.IsNullOrWhiteSpace(company.KPP)
                                        ? null
                                        : new XElement("КПП",
                                            System.Text.RegularExpressions.Regex
                                                .Replace(company.KPP, "[^0-9]", "")),
                                    new XElement("ЮридическоеФизическоеЛицо",
                                        company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                                new XElement("ВалютаВзаиморасчетов",
                                    new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                    new XElement("Код", 643)),
                                new XElement("РасчетыВУсловныхЕдиницах", false))))
                ));

                // платежи по киви
                documents.Add(new XElement("Документ.ОплатаПлатежнойКартой",
                    new XElement("КлючевыеСвойства",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                        new XElement("Дата", edate),
                        new XElement("Номер", i + "-" + sdate.ToString("MM.yyyy")),
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо"))),
                    new XElement("Ответственный",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                        new XElement("Наименование", "&lt;Не указан&gt;")),
                    new XElement("ВидОперации", "ОплатаПокупателя"),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", item.Key.Remove(1, 5) + "1" + sdate.ToString("MMyyyy")),
                        new XElement("Код", 643)),
                    new XElement("СуммаДокумента", item.Value.add_qiwi),
                    new XElement("Контрагент",
                        new XElement("Ссылка", "123"),
                        new XElement("НаименованиеПолное", "КИВИ БАНК (АО)"),
                        new XElement("ИНН", "3123011520"),
                        new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                    new XElement("ДанныеВзаиморасчетов",
                        new XElement("Договор",
                            new XElement("Ссылка", "123"),
                            new XElement("Номер", "Эквайринг"),
                            new XElement("Дата", "01.09.2015"),
                            new XElement("ВидДоговора", "Прочее"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "КИВИ БАНК (АО)"),
                                new XElement("ИНН", "3123011520"),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false)),
                        new XElement("ВалютаВзаиморасчетов",
                            new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                            new XElement("Код", 643)),
                        new XElement("КурсВзаиморасчетов", 1),
                        new XElement("КратностьВзаиморасчетов", 1),
                        new XElement("РасчетыВУсловныхЕдиницах", false)),
                    new XElement("ВидОплаты",
                        new XElement("Организация",
                            new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                            new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                            new XElement("НаименованиеПолное", НаименованиеПолное),
                            new XElement("ИНН", ИНН),
                            new XElement("КПП", КПП),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                        new XElement("ТипОплаты", "ПлатежнаяКарта"),
                        new XElement("Контрагент",
                            new XElement("Ссылка", "123"),
                            new XElement("НаименованиеПолное", "КИВИ БАНК (АО)"),
                            new XElement("ИНН", "3123011520"),
                            new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                        new XElement("ДоговорКонтрагента",
                            new XElement("Ссылка", item.Key.Remove(1, 5) + "5" + sdate.ToString("MMyyyy")),
                            new XElement("Дата", edate),
                            new XElement("Номер", "Эквайринг"),
                            new XElement("ВидДоговора", "Прочее"),
                            new XElement("Организация",
                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                new XElement("НаименованиеПолное", НаименованиеПолное),
                                new XElement("ИНН", ИНН),
                                new XElement("КПП", КПП),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("Контрагент",
                                new XElement("Ссылка", "123"),
                                new XElement("НаименованиеПолное", "КИВИ БАНК (АО)"),
                                new XElement("ИНН", "3123011520"),
                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                            new XElement("ВалютаВзаиморасчетов",
                                new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                new XElement("Код", 643)),
                            new XElement("РасчетыВУсловныхЕдиницах", false))),
                    new XElement("ВалютаДокумента",
                        new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                        new XElement("Код", 643)),
                    new XElement("РасшифровкаПлатежа",
                        new XElement("Строка",
                            new XElement("СуммаДокумента", item.Value.add_qiwi),
                            new XElement("КурсВзаиморасчетов", 1),
                            new XElement("КратностьВзаиморасчетов", 1),
                            new XElement("СпособПогашенияЗадолженности", "Автоматически"),
                            new XElement("СтавкаНДС", "БезНДС"),
                            new XElement("СчетУчетаРасчетовСКонтрагентом", "76.09"),
                            new XElement("СчетУчетаРасчетовПоАвансам", "76.09"),
                            new XElement("Контрагент",
                                new XElement("Ссылка", item.Key),
                                new XElement("НаименованиеСокращенное", company.OrgName),
                                new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                new XElement("ИНН",
                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                        "")),
                                string.IsNullOrWhiteSpace(company.KPP)
                                    ? null
                                    : new XElement("КПП",
                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
                                new XElement("ЮридическоеФизическоеЛицо",
                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                            new XElement("ДоговорКонтрагента",
                                new XElement("Ссылка", item.Key.Remove(1, 5) + "7" + sdate.ToString("MMyyyy")),
                                new XElement("Дата", edate),
                                new XElement("Номер", contract.Number),
                                new XElement("ВидДоговора", "СКомитентом"),
                                new XElement("Организация",
                                    new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
                                    new XElement("НаименованиеСокращенное", НаименованиеСокращенное),
                                    new XElement("НаименованиеПолное", НаименованиеПолное),
                                    new XElement("ИНН", ИНН),
                                    new XElement("КПП", КПП),
                                    new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),
                                new XElement("Контрагент",
                                    new XElement("Ссылка", item.Key),
                                    new XElement("НаименованиеСокращенное", company.OrgName),
                                    new XElement("НаименованиеПолное", company.OrgName + " " + company.OrgType),
                                    new XElement("ИНН",
                                        System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
                                            "")),
                                    string.IsNullOrWhiteSpace(company.KPP)
                                        ? null
                                        : new XElement("КПП",
                                            System.Text.RegularExpressions.Regex
                                                .Replace(company.KPP, "[^0-9]", "")),
                                    new XElement("ЮридическоеФизическоеЛицо",
                                        company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
                                new XElement("ВалютаВзаиморасчетов",
                                    new XElement("Ссылка", item.Key.Remove(1, 3) + "111"),
                                    new XElement("Код", 643)),
                                new XElement("РасчетыВУсловныхЕдиницах", false))))
                ));
            }

            if (i == 0)
                return;

            var head = @"
<Message xmlns:msg=""http://www.1c.ru/SSL/Exchange/Message"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	<msg:Header>
		<msg:Format>http://v8.1c.ru/edi/edi_stnd/EnterpriseData/1.0</msg:Format>
		<msg:CreationDate>2015-09-28T14:55:15</msg:CreationDate>
		<msg:Confirmation>
			<msg:ExchangePlan>СинхронизацияДанныхЧерезУниверсальныйФормат</msg:ExchangePlan>
			<msg:To>БП</msg:To>
			<msg:From>QQ</msg:From>
			<msg:MessageNo>4</msg:MessageNo>
			<msg:ReceivedNo>5</msg:ReceivedNo>
		</msg:Confirmation>
		<msg:AvailableVersion>1.0.beta</msg:AvailableVersion>
		<msg:AvailableVersion>1.0</msg:AvailableVersion>
	</msg:Header>
";

            var body = new XElement("Body", new XElement("Documents", documents));
            var xml = "<Body xmlns=\"http://v8.1c.ru/edi/edi_stnd/EnterpriseData/1.0\">" + new XDocument(body).ToString().Remove(0, "<Body>".Length);
            var file = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + head + xml + "</Message>";
            var filename = Path.Combine(syncFolder, string.Format("{0}.xml", agg.City));
            File.WriteAllText(filename, file);
        }

//        [Route("clients/item")]
//        public async Task<ActionResult> ClientsItem(string agg)
//        {
//            var body = new ConcurrentBag<XElement>();

//            await (await AggregatorHelper.Domain.ListAsync(agg)).ParallelForEach(4, async db =>
//            {
//                var company = await AggregatorHelper.Company.GetAsync(agg, db);
//                if (company == null) return;

//                var companyAgg = await AggregatorHelper.Company.GetAsync(agg, agg);

//                body.Add(new XElement("Справочник.Контрагенты",
//                    new XElement("КлючевыеСвойства",
//                        new XElement("Ссылка", db),
//                        new XElement("НаименованиеПолное", company.OrgType + " " + company.OrgName),
//                        new XElement("ИНН",
//                            System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]", "")),
//                        string.IsNullOrWhiteSpace(company.KPP)
//                            ? null
//                            : new XElement("КПП",
//                                System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
//                        new XElement("ЮридическоеФизическоеЛицо",
//                            company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),
//                    new XElement("Наименование", company.OrgName + " " + company.OrgType),
//                    new XElement("ОбособленноеПодразделение", false),
//                    new XElement("КонтактнаяИнформация",
//                        new XElement("Строка",
//                            new XElement("ВидКонтактнойИнформации", "АдресЭлектроннойПочты"),
//                            new XElement("ЗначенияПолей",
//                                new XElement("КонтактнаяИнформация",
//                                    new XAttribute("Представление", company.Email),
//                                    new XElement("Комментарий"),
//                                    new XElement("Состав",
//                                        new XAttribute("Значение", company.Email))))))
//                ));

//                if (!string.IsNullOrWhiteSpace(company.OrgAccount))
//                {
//                    body.Add(new XElement("Справочник.БанковскиеСчета",
//                        new XElement("КлючевыеСвойства",
//                            new XElement("Ссылка", db),
//                            new XElement("НомерСчета",
//                                System.Text.RegularExpressions.Regex.Replace((company.OrgAccount ?? ""), "[^0-9]", "")),
//                            new XElement("Банк",
//                                new XElement("Ссылка", db),
//                                new XElement("БИК",
//                                    System.Text.RegularExpressions.Regex.Replace((company.BIK ?? ""), "[^0-9]", "")),
//                                new XElement("КоррСчет",
//                                    System.Text.RegularExpressions.Regex.Replace((company.BankAccount ?? ""), "[^0-9]",
//                                        "")),
//                                new XElement("Наименование", (company.BankName ?? "").Trim())),
//                            new XElement("Владелец",
//                                new XElement("КонтрагентыСсылка",
//                                    new XElement("Ссылка", db),
//                                    new XElement("НаименованиеПолное", company.OrgType + " " + company.OrgName),
//                                    new XElement("ИНН",
//                                        System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]",
//                                            "")),
//                                    new XElement("ЮридическоеФизическоеЛицо",
//                                        company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")))),
//                        new XElement("Наименование", (company.BankName ?? "").Trim()),
//                        new XElement("ВалютаДенежныхСредств",
//                            new XElement("Ссылка", db),
//                            new XElement("Код", 643))
//                    ));
//                }

//                var contract = await AggregatorHelper.Contract.GetAsync(agg, db);
//                if (contract != null)
//                {
//                    // договор на киви
//                    body.Add(new XElement("Справочник.Договоры",
//                        new XElement("КлючевыеСвойства",
//                            new XElement("Ссылка", db),
//                            new XElement("ВидДоговора", ""),

//                            new XElement("Организация",
//                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
//                                new XElement("НаименованиеСокращенное", $"{companyAgg?.OrgType} \"{companyAgg?.OrgName}\""),
//                                new XElement("НаименованиеПолное", $"Общество с ограниченной ответственностью \"{companyAgg?.OrgName}\""),
//                                new XElement("ИНН", companyAgg?.INN),
//                                new XElement("КПП", companyAgg?.KPP),
//                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),

//                            new XElement("Контрагент",
//                                new XElement("Ссылка", db),
//                                new XElement("НаименованиеСокращенное", company.OrgType + " " + company.OrgName),
//                                new XElement("НаименованиеПолное", company.OrgType + " " + company.OrgName),
//                                new XElement("ИНН",
//                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]", "")),
//                                string.IsNullOrWhiteSpace(company.KPP)
//                                    ? null
//                                    : new XElement("КПП",
//                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
//                                new XElement("ЮридическоеФизическоеЛицо",
//                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),

//                            new XElement("ВалютаВзаиморасчетов",
//                                new XElement("Ссылка", db),
//                                new XElement("Код", 643)),
//                            new XElement("РасчетыВУсловныхЕдиницах", false)),

//                        new XElement("Наименование", "Эквайринг"),
//                        new XElement("Номер", "Эквайринг")
//                    ));

//                    body.Add(new XElement("Справочник.Договоры",
//                        new XElement("КлючевыеСвойства",
//                            new XElement("Ссылка", db),
//                            new XElement("ВидДоговора", "СКомитентомНаЗакупку"),

//                            new XElement("Организация",
//                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
//                                new XElement("НаименованиеСокращенное", $"{companyAgg?.OrgType} \"{companyAgg?.OrgName}\""),
//                                new XElement("НаименованиеПолное",
//                                    $"Общество с ограниченной ответственностью \"{companyAgg?.OrgName}\""),
//                                new XElement("ИНН", companyAgg?.INN),
//                                new XElement("КПП", companyAgg?.KPP),
//                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),

//                            new XElement("Контрагент",
//                                new XElement("Ссылка", db),
//                                new XElement("НаименованиеСокращенное", company.OrgType + " " + company.OrgName),
//                                new XElement("НаименованиеПолное", company.OrgType + " " + company.OrgName),
//                                new XElement("ИНН",
//                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]", "")),
//                                string.IsNullOrWhiteSpace(company.KPP)
//                                    ? null
//                                    : new XElement("КПП",
//                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
//                                new XElement("ЮридическоеФизическоеЛицо",
//                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),

//                            new XElement("ВалютаВзаиморасчетов",
//                                new XElement("Ссылка", db),
//                                new XElement("Код", 643)),
//                            new XElement("РасчетыВУсловныхЕдиницах", false)),

//                        new XElement("Наименование", contract.Number),
//                        new XElement("Номер", contract.Number)
//                    ));

//                    // 2-й договор
//                    body.Add(new XElement("Справочник.Договоры",
//                        new XElement("КлючевыеСвойства",
//                            new XElement("Ссылка", db.Remove(1, 3) + "999"),
//                            new XElement("ВидДоговора", "СКомитентом"),

//                            new XElement("Организация",
//                                new XElement("Ссылка", "bb1b502c-2625-11e5-8261-acb57d2696be"),
//                                new XElement("НаименованиеСокращенное", $"{companyAgg?.OrgType} \"{companyAgg?.OrgName}\""),
//                                new XElement("НаименованиеПолное",
//                                    $"Общество с ограниченной ответственностью \"{companyAgg?.OrgName}\""),
//                                new XElement("ИНН", companyAgg?.INN),
//                                new XElement("КПП", companyAgg?.KPP),
//                                new XElement("ЮридическоеФизическоеЛицо", "ЮридическоеЛицо")),

//                            new XElement("Контрагент",
//                                new XElement("Ссылка", db),
//                                new XElement("НаименованиеСокращенное", company.OrgType + " " + company.OrgName),
//                                new XElement("НаименованиеПолное", company.OrgType + " " + company.OrgName),
//                                new XElement("ИНН",
//                                    System.Text.RegularExpressions.Regex.Replace((company.INN ?? ""), "[^0-9]", "")),
//                                string.IsNullOrWhiteSpace(company.KPP)
//                                    ? null
//                                    : new XElement("КПП",
//                                        System.Text.RegularExpressions.Regex.Replace(company.KPP, "[^0-9]", "")),
//                                new XElement("ЮридическоеФизическоеЛицо",
//                                    company.OrgType == "ИП" ? "ФизическоеЛицо" : "ЮридическоеЛицо")),

//                            new XElement("ВалютаВзаиморасчетов",
//                                new XElement("Ссылка", db),
//                                new XElement("Код", 643)),
//                            new XElement("РасчетыВУсловныхЕдиницах", false)),

//                        new XElement("Наименование", contract.Number),
//                        new XElement("Номер", contract.Number)
//                    ));
//                }
//            });

//            var head = @"
//<msg:Header>
//	<msg:Format>http://v8.1c.ru/edi/edi_stnd/EnterpriseData/1.0</msg:Format>
//	<msg:CreationDate>2015-09-25T14:53:54</msg:CreationDate>
//	<msg:Confirmation>
//		<msg:ExchangePlan>СинхронизацияДанныхЧерезУниверсальныйФормат</msg:ExchangePlan>
//		<msg:To>БП</msg:To>
//		<msg:From>TX</msg:From>
//		<msg:MessageNo>51</msg:MessageNo>
//		<msg:ReceivedNo>50</msg:ReceivedNo>
//	</msg:Confirmation>
//	<msg:AvailableVersion>1.0.beta</msg:AvailableVersion>
//	<msg:AvailableVersion>1.0</msg:AvailableVersion>
//</msg:Header>
//";

//            var xbody = new XElement("Body", body);

//            var xml = new XDocument(xbody).ToString();

//            xml = "<Body xmlns=\"http://v8.1c.ru/edi/edi_stnd/EnterpriseData/1.0\">" + xml.Remove(0, "<Body>".Length);

//            xml = xml.Replace("<КонтактнаяИнформация", "<КонтактнаяИнформация xmlns=\"http://www.v8.1c.ru/ssl/contactinfo\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
//            xml = xml.Replace("<Состав", "<Состав xsi:type=\"ЭлектроннаяПочта\" ");

//            return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
//                           "<Message xmlns:msg=\"http://www.1c.ru/SSL/Exchange/Message\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
//                           head + xml + "</Message>", "text/xml", System.Text.Encoding.UTF8);
//        }
    }
}
