using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.WebForms.ModelBinding.Extensions.Samples.Model
{
    public class Product
    {
        public int ID { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }
        
        public double UnitPrice { get; set; }
        
        public int StockOnHand { get; set; }
        
        public bool InStock { get { return StockOnHand > 0; } }
    }
}