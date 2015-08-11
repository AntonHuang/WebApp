using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    public interface ICustomer
    {
        string CustomerID { get; set; }
        string CustomerType { get; set; }
    }
}
