﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Product;

namespace WebApp.DomainModels.Customer
{
    public class MemberPoint
    {
      

        public string ID { get; set; }

        public string OwnerMemberID { get; set; }
        public virtual Member Owner { get; set; }
        public decimal Quantity { get; set; }
        public decimal CurrentTotalQuantity { get; set; }
        public string Type { get; set; }
        public string Describe { get; set; }
        public DateTime CreateDate { get; set; }
        public string State { get; set; }

        public string OperationByMemberID { get; set; }
        public virtual Member OperationBy { get; set; }

        public DateTime DealDate { get; set; }
        public DateTime UseableDate { get; set; }

        public string ProductID { get; set; }
        public virtual Mattress Product { get; set; }

        public string ProductBuyerMemberID { get; set; }
        public virtual Member ProductBuyer { get; set; }

        public MemberPoint()
        {
            Task.Delay(1);
            CreateDate = DateTime.Now;
            DealDate = DateTime.Now;
            UseableDate = DateTime.Today.Date;
            State = "New";
        }

        public MemberPoint(SaleToCustomerDetail saleDetail) :this() {
            Product = saleDetail.Prodect;
            ProductBuyer = saleDetail.Sale.Customer;
            DealDate = saleDetail.Sale.DealDate;
            OperationBy = saleDetail.Sale.SellingAgents;
            Type = "FromSale";
        }
    }

    public enum LevelRelation { Self, Son, Grandson }

    public class LevelRule
    {
        public Amount SelfRate { get; set; }
        public Amount SonRate { get; set; }
        public Amount GrandsonRate { get; set; }

        public LevelRule()
        {

        }

        public Amount GetRate(LevelRelation relation) {
            switch (relation) {
                case LevelRelation.Self:
                    return SelfRate;
                case LevelRelation.Son:
                    return SonRate;
                case LevelRelation.Grandson:
                    return GrandsonRate;
                default:
                    return null;
            }
        }
    }

    public class MemberPointRule
    {
       
        public LevelRule Level0;
        public LevelRule Level1;
        public TimeSpan AvailableAfter;

        public MemberPointRule()
        {

        }

        public static MemberPointRule Default
        {
            get
            {
                return new MemberPointRule
                {
                    Level0 = new LevelRule
                    {
                        SelfRate = new Amount("%", 2),
                        SonRate = new Amount("%", 2),
                        GrandsonRate = new Amount("%", 2),
                    },
                    Level1 = new LevelRule()
                    {
                        SelfRate = new Amount("%", 20)
                    },
                    AvailableAfter = TimeSpan.FromDays(180)
                };
            }
            private set { }
        }

        public static MemberPointRule fromJson(string ruleSettingValue)
        {
            if (string.IsNullOrWhiteSpace(ruleSettingValue))
            {
                return MemberPointRule.Default;
            }
            else
            {
                return JsonConvert.DeserializeObject<MemberPointRule>(ruleSettingValue);
            }
        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public DateTime CalcAvailableDate(DateTime date) {
            return date.Add(AvailableAfter);
        }

        public decimal Calc(string level, LevelRelation relation, decimal qty)
        {
            Amount rate = GetRate(level, relation);
            if (rate == null)
            {
                return 0;
            }


            if ("%".Equals(rate.Type))
            {
                // rate = 20% 
                // qty = 200
                // result = (200 * 20) / 100  = 40 
 
                return (qty * rate.ValueOfNumber) / (new decimal(100));
            }
            return 0;
        }

        private Amount GetRate(string level, LevelRelation relation)
        {
            if ("Level0".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return getLevel0Rate(relation);
            }
            else if ("Level1".Equals(level, StringComparison.InvariantCultureIgnoreCase))
            {
                return getLevel1Rate(relation);
            }

            return null;
        }

        private Amount getLevel0Rate(LevelRelation relation)
        {
            return Level0 == null ? null : Level0.GetRate(relation);
        }

        private Amount getLevel1Rate(LevelRelation relation)
        {
            Amount result = Level1 == null ? null : Level1.GetRate(relation);
            if (result == null) {
                return getLevel0Rate(relation);
            }
            return result;
        }

    }

}
