using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AG.Core.Models
{
    public class AggregatorAct
    {
        public DateTime date { get; set; }

        public string agg { get; set; }
        public string db { get; set; }
        public string name { get; set; }
        public double qiwi_off { get; set; }

        public string db_name { get; set; }
        public string db_number { get; set; }
        public string db_city { get; set; }
        public string inn { get; set; }
        public double complete_cost { get; set; }

        // фактическая комиссия
        public double complete_commission_yandex_bill { get; set; }

        public double complete_commission_yandex { get; set; }

        //коммиссия за смену
        public double complete_commission_yandex_day { get; set; }
        public double complete_commission_broker { get; set; }
        public double complete_commission_rostaxi { get { return complete_commission_broker - complete_commission_yandex; } }

        public double cancel_commission_yandex { get; set; }
        public double cancel_commission_broker { get; set; }
        public double cancel_commission_rostaxi { get { return cancel_commission_broker - cancel_commission_yandex; } }

        public double start_balance { get; set; }
        public double end_balance { get; set; }

        public double add_balance { get; set; }

        public double add_balance_bank { get; set; }

        public double delete_balance { get; set; }

        public double add_qiwi { get; set; }

        public double add_card_pay { get; set; }
        public double delete_card_pay { get; set; }
        public double card_pay { get { return add_card_pay - delete_card_pay; } }

        public double add_tips_pay { get; set; }
        public double delete_tips_pay { get; set; }
        public double tips_pay { get { return add_tips_pay - delete_tips_pay; } }

        public double add_compensation_pay { get; set; }
        public double delete_compensation_pay { get; set; }
        public double compensation_pay { get { return add_compensation_pay - delete_compensation_pay; } }

        public double add_bonus_pay { get; set; }

        public double add_coupon_pay { get; set; }

        public double add_hand_pay { get; set; }
        public double delete_hand_pay { get; set; }
        public double hand_pay { get { return add_hand_pay - delete_hand_pay; } }

        public double add_corp_pay { get; set; }
        public double delete_corp_pay { get; set; }
        public double corp_pay { get { return add_corp_pay - delete_corp_pay; } }

        public double tanker { get; set; }

        public double complete_commission_with_workshift
        {
            get
            {
                return complete_commission_yandex + Math.Abs(workshift_pay_yandex);
            }
        }

        //смены
        public double workshift_pay_yandex { get; set; }

        public double workshift_pay_aggregator { get; set; }

        public double total
        {
            get
            {
                return complete_commission_broker + cancel_commission_broker +
                       add_balance + delete_balance + card_pay + tips_pay + compensation_pay + add_bonus_pay +
                       add_coupon_pay + hand_pay + corp_pay;
            }
        }

        public bool hide
        {
            get
            {
                return total == 0;
            }
        }

        public int Number
        {
            get
            {
                if (string.IsNullOrEmpty(db_number))
                    return int.MaxValue;

                var i = 0;
                var n = Regex.Replace(db_number, "[^0-9]", "");

                if (db_number.Contains("."))
                    n = db_number.Substring(0, db_number.IndexOf('.'));

                int.TryParse(n, out i);
                return i;
            }
        }
    }
}
