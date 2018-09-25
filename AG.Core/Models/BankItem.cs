using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{

    public class BankItem
    {
        public int Тип { get; set; }
        public DateTime Дата { get; set; }
        public int Номер { get; set; }
        public int ВидОперации { get; set; }
        public double Сумма { get; set; }
        public string ОснованиеПлатежа { get; set; }
        public string СчетПолучателя { get; set; }
        public string НаименованиеПолучателя { get; set; }
        public string ИННПолучателя { get; set; }
        public string НаименованиеБанкаПолучателя { get; set; }
    }

    public sealed class BankItemMap : ClassMap<BankItem>
    {
        public BankItemMap()
        {
            Map(m => m.Тип).Name("Тип");
            Map(m => m.Дата).Name("Дата");
            Map(m => m.Номер).Name("Номер");
            Map(m => m.ВидОперации).Name("Вид операции");
            Map(m => m.Сумма).Name("Сумма").TypeConverterOption.CultureInfo(CultureInfo.GetCultureInfo("en"));
            Map(m => m.ОснованиеПлатежа).Name("Основание платежа");
            Map(m => m.СчетПолучателя).Name("Счет Получателя");
            Map(m => m.НаименованиеПолучателя).Name("Наименование Получателя");
            Map(m => m.ИННПолучателя).Name("ИНН получателя");
            Map(m => m.НаименованиеБанкаПолучателя).Name("Наименование банка получателя");
        }
    }

    public sealed class BankItemWriterMap : ClassMap<BankItem>
    {
        public BankItemWriterMap()
        {
            Map(m => m.ИННПолучателя).Name("ИНН получателя");
            Map(m => m.НаименованиеПолучателя).Name("Наименование Получателя");
            Map(m => m.Дата).Name("Дата");
            Map(m => m.Номер).Name("Номер");
            Map(m => m.Сумма).Name("Сумма").TypeConverterOption.CultureInfo(CultureInfo.GetCultureInfo("en"));
            Map(m => m.ОснованиеПлатежа).Name("Основание платежа");
        }
    }



    public class ReportItem
    {
        public string Город { get; set; }
        public string ИНН { get; set; }
        public string Договор { get; set; }
        public string Наименование_Принципала { get; set; }
        public string логин_парка { get; set; }
        public double База { get; set; }
        public double долг_парка { get; set; }
        public double долг_АТ { get; set; }
        public double Удержана_комиссия_Яндекс { get; set; }
        public double Удержана_комиссия_АТ { get; set; }
        public double Покупка_смен { get; set; }
        public double Заправки { get; set; }
        public double Перечислено_парку { get; set; }
        public double Штрафы_Я { get; set; }
        public double Возвраты_Пользователям { get; set; }
        public double Ручные_возвраты_техподдержкой { get; set; }
        public double Пополнения_от_Принципала { get; set; }
        public double Пополнение_от_QIWI { get; set; }
        public double Возвраты_перечислений_парку { get; set; }
        public double Возвраты_прочие { get; set; }
        public double БН_заказы { get; set; }
        public double Корпаративные_заказы { get; set; }
        public double Cубсидии { get; set; }
        public double Купоны { get; set; }
        public double Компенсации { get; set; }
        public double Чаевые { get; set; }
        public double Долг_парка { get; set; }
        public double Долг_АТ { get; set; }

        public static ReportItem operator + (ReportItem a, ReportItem b) 
        {
            a.База += b.База;
            a.Удержана_комиссия_Яндекс += b.Удержана_комиссия_Яндекс;
            a.Удержана_комиссия_АТ += b.Удержана_комиссия_АТ;
            a.Покупка_смен += b.Покупка_смен;
            a.Заправки += b.Заправки;
            a.Перечислено_парку += b.Перечислено_парку;
            a.Штрафы_Я += b.Штрафы_Я;
            a.Возвраты_Пользователям += b.Возвраты_Пользователям;
            a.Ручные_возвраты_техподдержкой += b.Ручные_возвраты_техподдержкой;
            a.Пополнения_от_Принципала += b.Пополнения_от_Принципала;
            a.Пополнение_от_QIWI += b.Пополнение_от_QIWI;
            a.Возвраты_перечислений_парку += b.Возвраты_перечислений_парку;
            a.Возвраты_прочие += b.Возвраты_прочие;
            a.БН_заказы += b.БН_заказы;
            a.Корпаративные_заказы += b.Корпаративные_заказы;
            a.Cубсидии += b.Cубсидии;
            a.Купоны += b.Купоны;
            a.Компенсации += b.Компенсации;
            a.Чаевые += b.Чаевые;
            a.Долг_парка = b.Долг_парка;
            a.Долг_АТ = b.Долг_АТ;
            return a;
        }
    }

    public sealed class ReportItemMap : ClassMap<ReportItem>
    {
        public ReportItemMap()
        {
            Map(m => m.Город).Index(0);
            Map(m => m.ИНН).Index(1);
            Map(m => m.Договор).Index(2);
            Map(m => m.Наименование_Принципала).Index(3);
            Map(m => m.логин_парка).Index(4);
            Map(m => m.База).Index(5);
            Map(m => m.долг_парка).Index(6).TypeConverter(new ReportItemConverter());
            Map(m => m.долг_АТ).Index(7).TypeConverter(new ReportItemConverter());
            Map(m => m.Удержана_комиссия_Яндекс).Index(8).TypeConverter(new ReportItemConverter());
            Map(m => m.Удержана_комиссия_АТ).Index(9).TypeConverter(new ReportItemConverter());
            Map(m => m.Покупка_смен).Index(10).TypeConverter(new ReportItemConverter());
            Map(m => m.Заправки).Index(11).TypeConverter(new ReportItemConverter());
            Map(m => m.Перечислено_парку).Index(12).TypeConverter(new ReportItemConverter());
            Map(m => m.Штрафы_Я).Index(13).TypeConverter(new ReportItemConverter());
            Map(m => m.Возвраты_Пользователям).Index(14).TypeConverter(new ReportItemConverter());
            Map(m => m.Ручные_возвраты_техподдержкой).Index(15).TypeConverter(new ReportItemConverter());
            Map(m => m.Пополнения_от_Принципала).Index(16).TypeConverter(new ReportItemConverter());
            Map(m => m.Пополнение_от_QIWI).Index(17).TypeConverter(new ReportItemConverter());
            Map(m => m.Возвраты_перечислений_парку).Index(18).TypeConverter(new ReportItemConverter());
            Map(m => m.Возвраты_прочие).Index(19).TypeConverter(new ReportItemConverter());
            Map(m => m.БН_заказы).Index(20).TypeConverter(new ReportItemConverter());
            Map(m => m.Корпаративные_заказы).Index(21).TypeConverter(new ReportItemConverter());
            Map(m => m.Cубсидии).Index(22).TypeConverter(new ReportItemConverter());
            Map(m => m.Купоны).Index(23).TypeConverter(new ReportItemConverter());
            Map(m => m.Компенсации).Index(24).TypeConverter(new ReportItemConverter());
            Map(m => m.Чаевые).Index(25).TypeConverter(new ReportItemConverter());
            Map(m => m.Долг_парка).Index(26).TypeConverter(new ReportItemConverter());
            Map(m => m.Долг_АТ).Index(27).TypeConverter(new ReportItemConverter());
        }
    }

    public class ReportItemConverter : ITypeConverter
    {
        public object ConvertFromString(string text, CsvHelper.IReaderRow row, MemberMapData memberMapData)
        {
            var result = 0d;
            if (double.TryParse(text, out result))
                return result;
            return 0d;
        }

        public string ConvertToString(object value, CsvHelper.IWriterRow row, MemberMapData memberMapData)
        {
            return 0d.Equals(value) ? "" : value.ToString();
        }
    }
}
