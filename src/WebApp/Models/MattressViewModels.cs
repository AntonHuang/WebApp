using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    
    public class SellToCustomerViewModel
    {
        public string MattressID { get; set; }
        public string MattressTypeID { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerID { get; set; }
        public DateTime SaleDate { get; set; }
        public string Gifts { get; set; }

    }

    
}
