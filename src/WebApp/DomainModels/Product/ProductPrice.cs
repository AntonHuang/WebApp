using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    /**
      <<moment-interval>>
    */
    public class ProductPrice
    {
        public string PriceQty;
        public string PriceUOM;
        public string Status;

        public ProductDesc Product;
        public IPricer Pricer;
        
    }

    /**
      <<Role>>
    */
    public interface IPricer
    {
        
    }
}
