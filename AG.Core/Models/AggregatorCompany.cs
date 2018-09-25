using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AG.Core.Models
{
    public class AggregatorCompany
    {
        public string OrgType { get; set; }
        public string OrgName { get; set; }
        public string OrgAddress { get; set; }
        public string OrgAddressIndex { get; set; }
        public string UrOrgAddress { get; set; }
        public string UrOrgAddressIndex { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string BIK { get; set; }
        public string ORGN { get; set; }
        public string BankCity { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string OrgAccount { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        //должность
        public string Post { get; set; }

        public string FaceAcceptDocument { get; set; }
        public string BaseDocument { get; set; }
        public string Nalog { get; set; }
    }
}