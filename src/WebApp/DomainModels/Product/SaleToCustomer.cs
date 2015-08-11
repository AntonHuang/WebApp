using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    /**
      <<moment-interval>>
    */
    public class SaleToCustomer
    {
        public string ID { get; set; }

        public virtual ICustomer Customer {get;set;}
        public virtual ISaleRepresentative Seller { get; set; }

        public virtual ICollection<SaleToCustomerDetail> DetailItems { get; set; }

        public DateTime DueDate { get; set; }
        public string State { get; set; }
    }


    public class SaleToCustomerDetail
    {
        public Amount Quantity { get; set; }
        public virtual ProductDesc ProdectDesc { get; set; }
        public string State { get; set; }
    }


}
