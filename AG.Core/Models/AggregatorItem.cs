using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.Core.Models
{
    public class AggregatorItem
    {
        public string Agg { get; set; }
        public string Clid { get; set; }
        public bool Enable { get; set; }
        public double? Commission { get; set; }

        public AggregatorCompany Requisites { get; set; }

        // название
        public string Name { get; set; }
        // город
        public string City { get; set; }
        // дата создания
        public DateTime DateCreate { get; set; }
        // минимальная сумма при которой опевещаем абонента о необходимоти пополнить счет 
        public double BalanceLimitAlert { get; set; }
        // почты техподдержки
        public string SupportMail { get; set; }

        // номер договора
        public string ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }

        //кто создал
        public string Owner { get; set; }

        //главный агрегатор
        public string Parent { get; set; }
    }
}
