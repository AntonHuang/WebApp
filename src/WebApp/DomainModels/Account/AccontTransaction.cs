using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Account
{
    /**
      <<moment-interval>>
    */
    public class AccontTransaction
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public ICollection<AccontTransactionDetail> Items { get; set; }

        public void Transaction(Account fromAccount, Account toAccount, Amount amount) {

            AccontTransactionDetail fromDetial = new AccontTransactionDetail()
            {
                transaction = this,
                account = fromAccount,
                qty = -amount,
                description = "Transaction From"
            };

            AccontTransactionDetail toDetial = new AccontTransactionDetail()
            {
                transaction = this,
                account = toAccount,
                qty = amount,
                description = "Transaction To"
            };

            Items.Add(fromDetial);
            Items.Add(toDetial);

            fromDetial.ApplyTransaction();
            toDetial.ApplyTransaction();
        }
    }

    /**
      <<moment-interval>>
    */
    public class AccontTransactionDetail
    {
        public AccontTransaction transaction;
        public Account account; 
        public Amount qty;
        public AmountExchangeRate rate;
        public string description;
        

        public void ApplyTransaction() {
            rate = RateService.GetExchangeRate(qty, account.GetAmountForExchangeParse());
            account.modifyAmount(rate.GetAmount(qty));
        }
    }

    public class AmountExchangeRate {
        public Amount From;
        public Amount To;
        public decimal Rate;

        public Amount GetAmount(Amount qty)
        {
            if (From.Type.Equals(qty.Type) == false) {
                throw new ArgumentException(
                    String.Format("Expect {0} Amount Type.But Argument Amount Type is {1}",
                         From.Type.ToString(), qty.Type.ToString() ));
            }
            return new Amount(To.Type, qty.ValueOfNumber * Rate );
        }
    }

    public class RateService
    {
        internal static AmountExchangeRate GetExchangeRate(Amount from, Amount to)
        {
            if (from.Type.Equals("MemberPiont") && to.Type.Equals("RMB"))
            {

                return new AmountExchangeRate()
                {
                    From = new Amount("MemberPiont"),
                    To = new Amount("RMB"),
                    Rate = 50
                };
            }
            else if (from.Type.Equals("RMB") && to.Type.Equals("MemberPiont"))
            {
                return new AmountExchangeRate()
                {
                    From = new Amount("RMB"),
                    To = new Amount("MemberPiont"),
                    Rate = 1/50
                };
            }
            else {

                return new AmountExchangeRate()
                {
                    From = new Amount( from.Type),
                    To = new Amount(to.Type),
                    Rate = 1
                };
            }
        }
    }

}
