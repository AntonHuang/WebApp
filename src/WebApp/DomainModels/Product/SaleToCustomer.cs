using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Customer;

namespace WebApp.DomainModels.Product
{
    /**
      <<moment-interval>>
    */
    public class SaleToCustomer
    {
        public int ID { get; set; }

        public virtual Member Customer {get;set;}
        public virtual Member SellingAgents { get; set; }

        public virtual ICollection<SaleToCustomerDetail> DetailItems { get; set; }

        public DateTime DueDate { get; set; }
        public string State { get; set; }
    }


    public class SaleToCustomerDetail
    {
        public SaleToCustomer Sale { get; set; }
        
        public virtual Mattress Prodect { get; set; }
        
        //public Amount Price { get; set; }

        public string DeliveryAddress { get; set; }
        public string Gifts { get; set; }
        public string State { get; set; }
    }


}
